using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;    
    
    /// <summary>
    /// Tämä luokka suorittaa matemaattisia operaatioita blokkien arvoilla, joita on luotu
    /// Tästä luokasta vielä puuttuu itse matemaattisten operaatioiden käsittely
    /// </summary>
    public class OperationBlock : OperationalBlocks<OperationBlock>
    {
        /// <summary>
        /// Kuinka monen erillisen muuttujan matemaattinen blokki on kyseessä. 2=kahden muuttujan (1+1), 3=kolmen muuttujan (2+1)
        /// </summary>
        private int operationblocktype;
        /// <summary>
        /// Kuinka monen erillisen muuttujan matemaattinen blokki on kyseessä. 2=kahden muuttujan (1+1), 3=kolmen muuttujan (2+1)
        /// </summary>
        public int OperationBlockType {
            get { return this.operationblocktype; }
        }      

        /// <summary> Luokan constructor, joka vastaa matemaattisten operaatioiden suorittamisesta objektien välillä.  </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="prohmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="objind">ObjectIndexer, referenssi ObjectIndexer luokkaan joka ylläpitää tietoja minkä tyyppisistä objekteista on kyse ja niiden UID tiedoista sekä objektin instanssin referenssistä </param>
        /// <param name="parentuid"> long, tämän objektin luoneen äitiobjektin UID </param>
        /// <param name="granparentuid"> long, tämän objektin luoneen äitiobjektin vanhemman UID </param>
        /// <param name="objuid"> long, tämän objektin oma uid </param> 
        /// <param name="operblocktype"> int, Kuinka monen erillisen muuttujan matemaattinen blokki on kyseessä. 2=kahden muuttujan (1+1), 3=kolmen muuttujan (2+1) </param>
        /// <param name="selectedhandl"> int, Se vaihtoehto, joka on valittu comparisonblock kohteessa Operator comboboxin valinnaksi </param>
        /// <param name="route"> int, RouteId, joka vastaa käytännössä AltRoute tietoa Slotlistalla </param>
        /// <param name="blockn"> string, Blokin nimi / Title - ei käytetä mihinkään, mutta antaa lisätietoa käyttäjälle, miksi blokki on luotu </param>
        /// <returns> {void} </returns>
        public OperationBlock(string kutsuja, ProgramHMI prghmi, ObjectIndexer objind, long objuid, long parentuid, long granparentuid, int operblocktype, int selectedhandl, int route, string blockn)
        {
            string functionname="->(OB)OperationBlock";
            this.operationblocktype=operblocktype;
            this.SelectedHandle=selectedhandl;
            this.BlockName=blockn;
            this.RouteId=route;
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.OwnUID=objuid;
            this.proghmi=prghmi;
            this.objectindexer=objind;

            this.BlockResultValue=new BlockAtomValue(); // Luodaan atomi, jota voidaan siirtää blokista toiseen. Tämä atomi on tarkoitettu vain ExecuteBlock toimintoa varten. Kahvojen atomit luodaan BlockHandles luokassa 

            this.handleconnuidlist = new IncomingHandlesStatus(kutsuja+functionname,this.proghmi,this.objectindexer,this.ParentUID, this.GranParentUID, false);

            this.blockhandles=new BlockHandles(kutsuja+functionname, this.OwnUID, this.ParentUID, this.proghmi, this.objectindexer); // Luodaan luokka, joka pitää yllä kahvojen tietoja            
        }

        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <param="parentuid"> long, parent UID numero, jolla etsitään MotherConnectionRectanglen objekti ja sen kautta tarkistetaan onko kaikki incoming Handlet saaneet arvonsa. Jos tämä parametri on -1, niin käytetään tämän luokan oletus parentuid tietoa - muussa tapauksessa käytetään annettua parentuid tietoa. </param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>
        public override IncomingHandlesStatus CheckIncomingHandles(string kutsuja, long UIDtoseek, long parentuid = -1)
        {
            string functionname = "->(OB)CheckIncomingHandles#1";
            return CheckIncomingHandlesBase(kutsuja+functionname, UIDtoseek, parentuid);
        }

        /// <summary>
        /// Tarkistaa tulokahvat ja päivittää IncomingHandlesStatus-objektin niiden kahvojen osalta, jotka eivät ole vielä saaneet alkuarvojaan.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle, jonka kahvoja käsitellään.</param>
        /// <param name="incominghandle">IncomingHandlesStatus, johon päivitetään kahvat, jotka eivät ole saaneet arvojaan. Jos tämä on null, niin käytetään objektin omaa this.handleconnuidlist IncomingHandlesStatus instanssia</param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>
        public override IncomingHandlesStatus CheckIncomingHandles(string kutsuja, MotherConnectionRectangle motherconnrect, IncomingHandlesStatus incominghandle = null)
        {
            string functionname = "->(OB)CheckIncomingHandles#2";
            return CheckIncomingHandlesBase(kutsuja+functionname, motherconnrect, incominghandle);
        }

        /// <summary>
        /// Lähettää tulokset eteenpäin jokaiselle "result" pään Connection instansseille.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle instanssi, josta Connection instanssit haetaan.</param>
        /// <result> {long} palauttaa 1, jos onnistui ja pääsi viimeiseen elementtiin listalla tai 2, jos ei yhtään kohdetta listalla. Palauttaa miinusmerkkisen arvon jos eteen tuli virhe </returns>
        public override long SendHandlesForward(string kutsuja, MotherConnectionRectangle motherconnrect)
        {
            string functionname = "->(OB)SendHandlesForward";
            return this.SendHandlesForwardProtected(kutsuja+functionname, motherconnrect, this.BlockResultValue);
        }          

        /// <summary>
        /// ExecuteBlock tekee sen mitä kukin blokki tekee ja toteuttaa kyseisen blokin toiminnan
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="motherconnrect"> MotherConnectionRectangle, se pääblokin luokan instanssin referenssi, jonka kautta saamme käytyä noutamassa tiedot siitä, mitä blokille on luotu</param>
        /// <param name="oneslot"> OneSlot, sen slotin referenssi, josta tietoja "saatetaan" hakea. Kyseisen slotin kautta on myös ObjectIndexerillä mahdollista päästä käsiksi koko ohjelman perusparametreihin. Tämä voidaan antaa myös null tietona, jos kyseessä on käyttäjän itsensä antama arvo, jolloin OneSlot objektin referenssiä ei tarvita </param>
        /// <returns>{int} palauttaa BlockAtomValue:n tyypin enum:in, jos onnistui asettamaan kohteen tälle blokille Result tiedoksi. Jos tulee virhe, niin palauttaa arvon, joka on < 0.</returns>
        public override int ExecuteBlock(string kutsuja, MotherConnectionRectangle motherconnrect, OneSlot oneslot = null)
        {
            string functionName = "->(OperationBlock)ExecuteBlock";
            int result = -1;

            try
            {
                // Hae vihreä kahva
                SortedList<long, BlockHandle> greenHandles = this.GetBlockHandlesByIterationClass(kutsuja + functionName, (int)ConnectionRectangles.connectionBoxType.GREEN_BOX_CHECK_VALUE_2);
                if (greenHandles.Count != 1)
                {
                    this.proghmi.sendError(kutsuja + functionName, "Invalid number of green handles.", -1364, 4, 4);
                    return -40;
                }

                // Hae keltaiset kahvat
                SortedList<long, BlockHandle> yellowHandles = GetBlockHandlesByIterationClass(kutsuja + functionName, (int)ConnectionRectangles.connectionBoxType.YELLOW_BOX_COMPARE_VALUE_1);
                if (yellowHandles.Count == 0)
                {
                    this.proghmi.sendError(kutsuja + functionName, "No yellow handles found.", -1365, 4, 4);
                    return -41;
                }

                // Hae arvot kahvoista
                BlockHandle greenHandle = greenHandles.Values.First();
                BlockAtomValue valueA = greenHandle.BlockAtomValue;
                List<BlockAtomValue> valuesB = yellowHandles.Values.Select(h => h.BlockAtomValue).ToList();

                // Tarkista atomityypit
                if (valuesB.Any(v => v.AtomType != valueA.AtomType))
                {
                    this.proghmi.sendError(kutsuja + functionName, "Atom types do not match.", -1366, 4, 4);
                    return -42;
                }

                // Hae operaattori
                string selectedHandleString = motherconnrect.SelectedHandle;
                int operationType = this.SwitchSelectedHandleStringToEnumInt(kutsuja + functionName, selectedHandleString);

                // Suorita toiminto atomityypin mukaan
                switch (valueA.AtomType)
                {
                    case (int)BlockAtomValue.AtomTypeEnum.Int:
                        this.BlockResultValue.IntAtom = PerformIntOperation(kutsuja + functionName, valueA.IntAtom, valuesB.Select(v => v.IntAtom).ToList(), operationType);
                        result=(int)BlockAtomValue.Int;
                        break;
                    case (int)BlockAtomValue.AtomTypeEnum.Long:
                        this.BlockResultValue.LongAtom = PerformLongOperation(kutsuja + functionName, valueA.LongAtom, valuesB.Select(v => v.LongAtom).ToList(), operationType);
                        result=(int)BlockAtomValue.Long;
                        break;
                    case (int)BlockAtomValue.AtomTypeEnum.Decimal:
                        this.BlockResultValue.DecAtom = PerformDecimalOperation(kutsuja + functionName, valueA.DecAtom, valuesB.Select(v => v.DecAtom).ToList(), operationType);
                        result=(int)BlockAtomValue.Dec;
                        break;
                    case (int)BlockAtomValue.AtomTypeEnum.String:
                        this.BlockResultValue.StringAtom = PerformStringOperation(kutsuja + functionName, valueA.StringAtom, valuesB.Select(v => v.StringAtom).ToList(), operationType);
                        result=(int)BlockAtomValue.String;
                        break;
                    case (int)BlockAtomValue.AtomTypeEnum.Bool:
                        this.BlockResultValue.BoolAtom = PerformBoolOperation(kutsuja + functionName, valueA.BoolAtom, valuesB.Select(v => v.BoolAtom).ToList(), operationType);
                        result=(int)BlockAtomValue.Bool;
                        break;
                    default:
                        result=(int)BlockAtomValue.Not_defined_yet;
                        this.proghmi.sendError(kutsuja + functionName, "Unsupported AtomType. AtomType:"+valueA.AtomType, -1367, 4, 4);
                        return -43;
                }

            }
            catch (Exception ex)
            {
                this.proghmi.sendError(kutsuja + functionName, ex.Message, -1368, 4, 4);
                return -44;
            }

            return result;
        }

        /// <summary>
        /// Suorittaa matemaattisen operaation kokonaisluvuille (int).
        /// </summary>
        /// <param name="caller">string, kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="valueA">int, ensimmäinen arvo operaatiossa.</param>
        /// <param "valuesB">List&lt;int&gt;, lista muista arvoista operaatiossa.</param>
        /// <param name="operationType">int, operaation tyyppi SelectedHandleEnum mukaan.</param>
        /// <returns>Palauttaa operaation tuloksen int-muodossa.</returns>
        private int PerformIntOperation(string caller, int valueA, List<int> valuesB, int operationType)
        {
            string functionname="->(OB)PerformIntOperation";
            int result = valueA;
            foreach (int valueB in valuesB)
            {
                switch (operationType)
                {
                    case (int)this.SelectedHandleEnum.PLUS_10:
                        result += valueB;
                        break;
                    case (int)SelectedHandleEnum.MINUS_11:
                        result -= valueB;
                        break;
                    case (int)SelectedHandleEnum.MULTIPLIER_12:
                        result *= valueB;
                        break;
                    case SelectedHandleEnum.DIVIDER_13:
                        if (valueB == 0)
                        {
                            this.proghmi.sendError(caller+functionname, "Division by zero.", -1369, 4, 4);
                            throw new InvalidOperationException("Division by zero! (PerformIntOperation)");
                            return 0;
                        }
                        result /= valueB;
                        break;
                    default:
                        this.proghmi.sendError(caller+functionname, "Invalid operation type for int operation: " + operationType, -1370, 4, 4);
                        throw new InvalidOperationException("Invalid operation type for int operation: " + operationType);
                }
            }
            return result;
        }

        /// <summary>
        /// Suorittaa matemaattisen operaation pitkille arvoille (long).
        /// </summary>
        /// <param name="caller">string, kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="valueA">long, ensimmäinen arvo operaatiossa.</param>
        /// <param name="valuesB">List&lt;long&gt;, lista muista arvoista operaatiossa.</param>
        /// <param name="operationType">int, operaation tyyppi SelectedHandleEnum mukaan.</param>
        /// <returns>Palauttaa operaation tuloksen long-muodossa.</returns>
        private long PerformLongOperation(string caller, long valueA, List<long> valuesB, int operationType)
        {
            string functionName = "->(OB)PerformLongOperation";
            long result = valueA;

            foreach (long valueB in valuesB)
            {
                switch (operationType)
                {
                    case (int)SelectedHandleEnum.PLUS_10:
                        result += valueB;
                        break;
                    case (int)SelectedHandleEnum.MINUS_11:
                        result -= valueB;
                        break;
                    case (int)SelectedHandleEnum.MULTIPLIER_12:
                        result *= valueB;
                        break;
                    case (int)SelectedHandleEnum.DIVIDER_13:
                        if (valueB == 0)
                        {
                            this.proghmi.sendError(caller + functionName, "Division by zero.", -1374, 4, 4);
                            throw new InvalidOperationException("Division by zero! (PerformLongOperation)");
                        }
                        result /= valueB;
                        break;
                    default:
                        this.proghmi.sendError(caller + functionName, "Invalid operation type for long operation: " + operationType, -1375, 4, 4);
                        throw new InvalidOperationException("Invalid operation type for long operation: " + operationType);
                }
            }
            return result;
        }

        /// <summary>
        /// Suorittaa matemaattisen operaation desimaaliluvuille (decimal).
        /// </summary>
        /// <param name="caller">string, kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="valueA">decimal, ensimmäinen arvo operaatiossa.</param>
        /// <param name="valuesB">List&lt;decimal&gt;, lista muista arvoista operaatiossa.</param>
        /// <param name="operationType">int, operaation tyyppi SelectedHandleEnum mukaan.</param>
        /// <returns>Palauttaa operaation tuloksen decimal-muodossa.</returns>
        private decimal PerformDecimalOperation(string caller, decimal valueA, List<decimal> valuesB, int operationType)
        {
            string functionName = "->(OB)PerformDecimalOperation";
            decimal result = valueA;

            foreach (decimal valueB in valuesB)
            {
                switch (operationType)
                {
                    case (int)SelectedHandleEnum.PLUS_10:
                        result += valueB;
                        break;
                    case (int)SelectedHandleEnum.MINUS_11:
                        result -= valueB;
                        break;
                    case (int)SelectedHandleEnum.MULTIPLIER_12:
                        result *= valueB;
                        break;
                    case (int)SelectedHandleEnum.DIVIDER_13:
                        if (valueB == 0)
                        {
                            this.proghmi.sendError(caller + functionName, "Division by zero.", -1376, 4, 4);
                            throw new InvalidOperationException("Division by zero! (PerformDecimalOperation)");
                        }
                        result /= valueB;
                        break;
                    default:
                        this.proghmi.sendError(caller + functionName, "Invalid operation type for decimal operation: " + operationType, -1377, 4, 4);
                        throw new InvalidOperationException("Invalid operation type for decimal operation: " + operationType);
                }
            }
            return result;
        }

        /// <summary>
        /// Suorittaa matemaattisen operaation merkkijonoille (string).
        /// </summary>
        /// <param name="caller">string, kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="valueA">string, ensimmäinen arvo operaatiossa.</param>
        /// <param name="valuesB">List&lt;string&gt;, lista muista arvoista operaatiossa.</param>
        /// <param name="operationType">int, operaation tyyppi SelectedHandleEnum mukaan.</param>
        /// <returns>Palauttaa 1, jos operaatio onnistui.</returns>
        private string PerformStringOperation(string caller, string valueA, List<string> valuesB, int operationType)
        {
            string functionName = "->(OB)PerformStringOperation";
            string result = valueA;

            foreach (string valueB in valuesB)
            {
                if (operationType == (int)SelectedHandleEnum.PLUS_10)
                {
                    result += valueB;
                }
                else
                {
                    this.proghmi.sendError(caller + functionName, "Invalid operation type for string operation: " + operationType, -1378, 4, 4);
                    throw new InvalidOperationException("Invalid operation type for string operation: " + operationType);
                }
            }
            this.BlockAtomResult.StringResult = result;
            return result;
        }


        /// <summary>
        /// Suorittaa matemaattisen operaation totuusarvoille (bool).
        /// </summary>
        /// <param name="caller">string, kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="valueA">bool, ensimmäinen arvo operaatiossa.</param>
        /// <param name="valuesB">List&lt;bool&gt;, lista muista arvoista operaatiossa.</param>
        /// <param name="operationType">int, operaation tyyppi SelectedHandleEnum mukaan.</param>
        /// <returns>Palauttaa true, jos operaatio onnistui ja tulos on true; false, jos tulos on false.</returns>
        private bool PerformBoolOperation(string caller, bool valueA, List<bool> valuesB, int operationType)
        {
            string functionName = "->(OB)PerformBoolOperation";
            int result = valueA ? 1 : 0;

            foreach (bool valueB in valuesB)
            {
                if (operationType == (int)SelectedHandleEnum.MULTIPLIER_12)
                {
                    result *= (valueB ? 1 : 0);
                }
                else
                {
                    this.proghmi.sendError(caller + functionName, "Invalid operation type for bool operation: " + operationType, -1379, 4, 4);
                    throw new InvalidOperationException("Invalid operation type for bool operation: " + operationType);
                }
            }
            this.BlockAtomResult.BoolResult = (result == 1);
            return (result == 1);
        }

        /// <summary>
        /// Tämä enumeraatio määrittää kuinka monen erillisen muuttujan matemaattinen blokki on kyseessä
        /// </summary>
        public enum OperationBlockTypeEnum {
            /// <summary>
            /// Normaali matemaattinen operaatio kahden luvun välillä
            /// </summary>
            NORMAL_OPERATION_2=2,
            /// <summary>
            /// Kolmen luvun matemaattinen operaation siten, että erilliselle tekijälle tapahtuvat kaksi seuraavaa matemaattista operaatiota jotka ovat tyypiltään samanlaisia, esim. 5-1-2 tai 5/2/3, mutta tällä blokilla ei voi tehdä esim. 5-1+2, vaan siihen tarvitaan 2 erillistä blokkia
            /// </summary>
            TWO_PLUS_ONE_OPERATION_3=3
        }

        /// <summary>
        /// Tämä enumeraatio määrittää mikä on minkäkin valitun operaattorin tyyppi
        /// </summary>
        public enum SelectedHandleEnum {
            /// <summary> Valittu kohde ei ole mikään tämän enumeraation virallisista kohteista </summary>
            ERROR_SELECTION_MINUS_1=-1,
            /// <summary>  Kohdetta ei ole vielä asetettu </summary>
            HANDLE_NOT_SET_0=0,
            /// <summary> Vastaa + merkintää </summary>
            PLUS_10=10,
            /// <summary> Vastaa - merkintää </summary>
            MINUS_11=11,
            /// <summary> Vastaa * merkintää </summary>
            MULTIPLIER_12=12,
            /// <summary> Vastaa / merkintää </summary>
            DIVIDER_13=13
        }

        /// <summary>
        /// Tämä metodi ottaa sisäänsä selectedhandlestring muuttujan ja vertaa sitä mahdollisiin comboboxiin tallennettuihin vaihtoehtoihin ja palauttaa SelectedHandleEnum enumeraatiota vastaavan vaihtoehdon int muotoisena
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="selectedhandlestring">string, Combobox tyyppisen UI elementin sisältö SelectedHandleCombobox:sta</param>
        /// <returns> {int} palauttaa SelectedHandleEnum enumeraation vaihtoehdon int muodossa </returns>
        public int SwitchSelectedHandleStringToEnumInt(string kutsuja, string selectedhandlestring)
        {
            string functionname="->(OB)SwitchSelectedHandleStringToEnumInt";
            switch (selectedhandlestring)
            {
                case "": return (int)SelectedHandleEnum.HANDLE_NOT_SET_0;
                case "+": return (int)SelectedHandleEnum.PLUS_10; 
                case "-": return (int)SelectedHandleEnum.MINUS_11; 
                case "*": return (int)SelectedHandleEnum.MULTIPLIER_12;
                case "/": return (int)SelectedHandleEnum.DIVIDER_13;

                default: 
                    // Log error or handle unknown string
                    this.proghmi.sendError(kutsuja+functionname," Unknown selected handle string: "+selectedhandlestring,-1099,4,4);
                    return (int)SelectedHandleEnum.ERROR_SELECTION_MINUS_1; // Virhekoodi -1 tuntemattomille merkkijonoille
            }
        }                
    }