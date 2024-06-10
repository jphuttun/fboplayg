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