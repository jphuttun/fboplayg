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

    /// <summary> Tämä luokka pitää sisällään yksittäiset vertailuatomit, kuten esim. pienempi kuin, suurempi kuin, yhtäsuuri, erisuuri, välissä, ulkopuolella jne. </summary>
    public class ComparisonBlock : OperationalBlocks<ComparisonBlock>
    {
        /// <summary> Minkä tyyppinen comparison block on kyseessä. 1=normaali valinta, 2=between/outside tyyppinen tarkastelut </summary>
        private int comparisonblocktype;

        /// <summary> Minkä tyyppinen comparison block on kyseessä. 1=normaali valinta, 2=between/outside tyyppinen tarkastelut </summary>
        public int ComparisonBlockType {
            get { return this.comparisonblocktype; }
        }

        /// <summary>
        /// BlockAtomValue silloin, kun tieto lähetetään false handlesta
        /// </summary>
        private BlockAtomValue inverseBlockResultValue;

        /// <summary>
        /// BlockAtomValue silloin, kun tieto lähetetään true handlesta
        /// </summary>
        private BlockAtomValue comptrueBlockResultValue;    

        /// <summary> Luokan constructor, joka vastaa valintablokeista, eli tekee tyypillisiä IF vertailuja. Lopputulemana on 1, jos vertailu on true ja 0, jos vertailu on false. </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="prohmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="objind">ObjectIndexer, referenssi ObjectIndexer luokkaan joka ylläpitää tietoja minkä tyyppisistä objekteista on kyse ja niiden UID tiedoista sekä objektin instanssin referenssistä </param>
        /// <param name="comparisonparentuid"> long, tämän objektin luoneen äitiobjektin UID </param>
        /// <param name="comparisongranparentuid"> long, tämän objektin luoneen äitiobjektin vanhemman UID </param>
        /// <param name="comparisonblockuid"> long, tämän objektin oma uid </param>
        /// <param name="comparisonbtype"> int, Minkä tyyppinen comparison block on kyseessä. 1=normaali valinta, 2=between/outside tyyppinen tarkastelut </param>
        /// <param name="selectedhandl"> int, Se vaihtoehto, joka on valittu comparisonblock kohteessa Operator comboboxin valinnaksi </param>
        /// <param name="route"> int, RouteId, joka vastaa käytännössä AltRoute tietoa Slotlistalla </param>
        /// <param name="blockn"> string, Blokin nimi / Title - ei käytetä mihinkään, mutta antaa lisätietoa käyttäjälle, miksi blokki on luotu </param>
        /// <returns> {void} </returns>
        public ComparisonBlock(string kutsuja, ProgramHMI prohmi, ObjectIndexer objind, long comparisonblockuid, long comparisonparentuid, long comparisongranparentuid, int comparisonbtype, int selectedhandl, int route, string blockn)
        {
            string functionname="->(CB)ComparisonBlock";
            this.proghmi=prohmi;
            this.objectindexer=objind;
            this.OwnUID=comparisonblockuid;
            this.ParentUID=comparisonparentuid;
            this.GranParentUID=comparisongranparentuid;
            this.comparisonblocktype=comparisonbtype;
            this.SelectedHandle=selectedhandl;
            this.RouteId=route;
            this.BlockName=blockn;

            this.BlockResultValue=new BlockAtomValue(); // Luodaan atomi, jota voidaan siirtää blokista toiseen. Tämä atomi on tarkoitettu vain ExecuteBlock toimintoa varten. Kahvojen atomit luodaan BlockHandles luokassa
            this.comptrueBlockResultValue = new BlockAtomValue();
            this.inverseBlockResultValue = new BlockAtomValue(); 

            this.handleconnuidlist = new IncomingHandlesStatus(kutsuja+functionname,this.proghmi,this.objectindexer,this.ParentUID, this.GranParentUID, false);

            this.blockhandles=new BlockHandles(kutsuja+functionname, this.OwnUID, this.ParentUID, this.proghmi, this.objectindexer); // Luodaan luokka, joka pitää yllä kahvojen tietoja            

        }

        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <param name="parentuid"> long, parent UID numero, jolla etsitään MotherConnectionRectanglen objekti ja sen kautta tarkistetaan onko kaikki incoming Handlet saaneet arvonsa. Jos tämä parametri on -1, niin käytetään tämän luokan oletus parentuid tietoa - muussa tapauksessa käytetään annettua parentuid tietoa. </param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>
        public override IncomingHandlesStatus CheckIncomingHandles(string kutsuja, long UIDtoseek, long parentuid = -1)
        {
            string functionname = "->(CB)CheckIncomingHandles#1";
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
            string functionname = "->(CB)CheckIncomingHandles#2";
            return CheckIncomingHandlesBase(kutsuja+functionname, motherconnrect, incominghandle);
        }        

        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <param name="parentuid"> long parent UID numero, jolla etsitään MotherConnectionRectanglen objekti ja sen kautta tarkistetaan onko kaikki incoming Handlet saaneet arvonsa. Jos tämä parametri on -1, niin käytetään tämän luokan oletus parentuid tietoa - muussa tapauksessa käytetään annettua parentuid tietoa. </param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>

        /*        public override IncomingHandlesStatus CheckIncomingHandles(string kutsuja, long UIDtoseek, long parentuid=-1)
        {
            string functionname="->(CB)CheckIncomingHandles#1";
            return this.CheckIncomingHandlesProtected(kutsuja+functionname,UIDtoseek,parentuid); // Pyöräytetään CheckIncomingHandles metodin toteutus takaisin pääfunktiolle, jos se on toteutettu normaalisti. Mikäli kyseessä on esim. ValueBlock, jossa ei ole lainkaan tulopuolen kahvoja, niin tällöin toteutus on erilainen ja kyseisessä tapauksessa ei kutsuta pääluokan vastaavaa metodia vaan palautetaan luokan instanssi, jonne on merkitty, että enumeraatiolla ettei kahvoja ole
        } */

        /// <summary>
        /// Tarkistaa tulokahvat ja päivittää IncomingHandlesStatus-objektin niiden kahvojen osalta, jotka eivät ole vielä saaneet alkuarvojaan.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle, jonka kahvoja käsitellään.</param>
        /// <param name="incominghandle">IncomingHandlesStatus, johon päivitetään kahvat, jotka eivät ole saaneet arvojaan. Jos tämä on null, niin käytetään objektin omaa this.handleconnuidlist IncomingHandlesStatus instanssia</param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>        
        /* public override IncomingHandlesStatus CheckIncomingHandles(string kutsuja, MotherConnectionRectangle motherconnrect, IncomingHandlesStatus incominghandle=null)
        {
            string functionname = "->(CB)CheckIncomingHandles#2";
            if (incominghandle==null) {
                return this.CheckIncomingHandlesProtected(kutsuja+functionname,motherconnrect,this.handleconnuidlist); // Pyöräytetään CheckIncomingHandles metodin toteutus takaisin pääfunktiolle, jos se on toteutettu normaalisti. Mikäli kyseessä on esim. ValueBlock, jossa ei ole lainkaan tulopuolen kahvoja, niin tällöin toteutus on erilainen ja kyseisessä tapauksessa ei kutsuta pääluokan vastaavaa metodia vaan palautetaan luokan instanssi, jonne on merkitty, että enumeraatiolla ettei kahvoja ole
            } else {
                return this.CheckIncomingHandlesProtected(kutsuja+functionname,motherconnrect,incominghandle); // Pyöräytetään CheckIncomingHandles metodin toteutus takaisin pääfunktiolle, jos se on toteutettu normaalisti. Mikäli kyseessä on esim. ValueBlock, jossa ei ole lainkaan tulopuolen kahvoja, niin tällöin toteutus on erilainen ja kyseisessä tapauksessa ei kutsuta pääluokan vastaavaa metodia vaan palautetaan luokan instanssi, jonne on merkitty, että enumeraatiolla ettei kahvoja ole                
            }
        } */

        /// <summary>
        /// Lähettää tulokset eteenpäin jokaiselle "result" pään Connection instansseille.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle instanssi, josta Connection instanssit haetaan.</param>
        /// <result> {long} palauttaa 1, jos onnistui ja pääsi viimeiseen elementtiin listalla tai 2, jos ei yhtään kohdetta listalla. Palauttaa miinusmerkkisen arvon jos eteen tuli virhe </returns>
        public override long SendHandlesForward(string kutsuja, MotherConnectionRectangle motherconnrect)
        {
            string functionname = "->(CB)SendHandlesForward";
            return this.SendHandlesForwardProtected(kutsuja+functionname, motherconnrect, this.blockhandles, (int)ConnectionRectangles.connectionBoxType.RED_BOX_RESULT_VALUE_3);
        }          

        /// <summary>
        /// Tämä enumeraatio määrittää kuinka tämän blokin verailu tehdään, kun käytetään eri vertailufunktioita
        /// </summary>
        public enum ComparisonBlockTypeEnum {
            /// <summary>
            /// Normaalin vertailun minimiarvo hyväksytylle SelectedHandle:n arvolle SelectedHandleEnum enumeraatiossa
            /// </summary>
            NORMAL_COMPARISON_MIN_INDEX_VALUE=10,
            /// <summary>
            /// Normaalin vertailun maksimiarvo hyväksytylle SelectedHandle:n arvolle SelectedHandleEnum enumeraatiossa
            /// </summary>            
            NORMAL_COMPARISON_MAX_INDEX_VALUE=15,
            /// <summary>
            /// Between/outside vertailun minimiarvo hyväksytylle SelectedHandle:n arvolle SelectedHandleEnum enumeraatiossa
            /// </summary>            
            BETWEEN_OUTSIDE_COMPARISON_MIN_INDEX_VALUE=110,
            /// <summary>
            /// Between/outside vertailun maksimiarvo hyväksytylle SelectedHandle:n arvolle SelectedHandleEnum enumeraatiossa
            /// </summary>            
            BETWEEN_OUTSIDE_COMPARISON_MAX_INDEX_VALUE=123,
            /// <summary>
            /// Normaali If tyyppinen vertailu, jossa katsotaan, onko jokin yhtäsuuri, erisuuri, suurempi, pienempi jne.
            /// </summary>
            NORMAL_COMPARISON_1=1,
            /// <summary>
            /// Between outside tyyppinen vertailu, jossa vertaillaan onko jokin luku joltain väliltä, esim. onko luku 0.5 väliltä 0 - 1 vai sen ulkopuolella. Tämä vertailu ei vielä toimi string tyyppisillä kohteilla
            /// </summary>
            BETWEEN_OUTSIDE_COMPARISON_2=2
        }

        /// <summary>
        /// Tämä enumeraatio määrittää mikä on minkäkin valitun operaattorin numero, esim. == saa luvun 10, != saa luvun 11 jne.
        /// </summary>
        public enum SelectedHandleEnum {
            /// <summary>
            /// Valittu kohde ei ole mikään tämän enumeraation virallisista kohteista
            /// </summary>
            ERROR_SELECTION_MINUS_1=-1,
            /// <summary>
            /// Kohdetta ei ole vielä asetettu
            /// </summary>
            HANDLE_NOT_SET_0=0,
            /// <summary> Vastaa == merkintää </summary>
            EQUAL_10=10,
            /// <summary> Vastaa != merkintää </summary>
            NOT_EQUAL_11=11,
            /// <summary> Vastaa suurempikuin merkintää </summary>
            ABOVE_12=12,
            /// <summary> Vastaa suurempi tai yhtäsuuri merkintää </summary>
            ABOVE_OR_EQUAL_13=13,
            /// <summary> Vastaa pienempi kuin merkintää </summary>
            BELOW_14=14,
            /// <summary> Vastaa pienempi tai yhtäsuuri merkintää </summary>
            BELOW_OR_EQUAL_15=15,
            /// <summary> Vastaa suurempi kuin between pienempi kuin merkintää </summary>
            BETWEEN_VALUES_110=110,
            /// <summary> Vastaa suurempi tai yhtäsuuri kuin between pienempi kuin merkintää </summary>
            BETWEEN_VALUES_OR_EQUAL_WITH_LOWER_BOUND_111=111,
            /// <summary> Vastaa suurempi kuin between pienempi tai yhtäsuuri kuin merkintää </summary>
            BETWEEN_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_112=112,
            /// <summary> Vastaa suurempi tai yhtäsuuri kuin between pienempi tai yhtäsuuri kuin merkintää </summary>
            BETWEEN_OR_EQUAL_WITH_BOUNDS_113=113,
            /// <summary> Vastaa suurempi kuin outside pienempi kuin merkintää </summary>
            OUTSIDE_VALUES_120=120,
            /// <summary> Vastaa Vastaa suurempi tai yhtäsuuri kuin outside pienempi kuin merkintää merkintää </summary>
            OUTSIDE_VALUES_OR_EQUAL_WITH_LOWER_BOUND_121=121,
            /// <summary> Vastaa suurempi tai yhtäsuuri kuin outside pienempi tai yhtäsuuri kuin merkintää </summary>
            OUTSIDE_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_122=122,
            /// <summary> Vastaa suurempi tai yhtäsuuri kuin outside pienempi tai yhtäsuuri kuin merkintää </summary>
            OUTSIDE_OR_EQUAL_WITH_BOUNDS_123=123
        }

        /// <summary>
        /// Tämä metodi ottaa sisäänsä selectedhandlestring muuttujan ja vertaa sitä mahdollisiin comboboxiin tallennettuihin vaihtoehtoihin ja palauttaa SelectedHandleEnum enumeraatiota vastaavan vaihtoehdon int muotoisena
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="selectedhandlestring">string, Combobox tyyppisen UI elementin sisältö SelectedHandleCombobox:sta</param>
        /// <returns> {int} palauttaa SelectedHandleEnum enumeraation vaihtoehdon int muodossa </returns>
        public int SwitchSelectedHandleStringToEnumInt(string kutsuja, string selectedhandlestring)
        {
            string functionname="->(CB)SwitchSelectedHandleStringToEnumInt";
            switch (selectedhandlestring)
            {
                case "": return (int)SelectedHandleEnum.HANDLE_NOT_SET_0;
                case "==": return (int)SelectedHandleEnum.EQUAL_10;
                case "!=": return (int)SelectedHandleEnum.NOT_EQUAL_11;
                case ">": return (int)SelectedHandleEnum.ABOVE_12;
                case ">=": return (int)SelectedHandleEnum.ABOVE_OR_EQUAL_13;
                case "<": return (int)SelectedHandleEnum.BELOW_14;
                case "<=": return (int)SelectedHandleEnum.BELOW_OR_EQUAL_15;

                case "> between <": return (int)SelectedHandleEnum.BETWEEN_VALUES_110;
                case ">= between <": return (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_LOWER_BOUND_111;
                case "> between <=": return (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_112;
                case ">= between <=": return (int)SelectedHandleEnum.BETWEEN_OR_EQUAL_WITH_BOUNDS_113;

                case "outside > < outside": return (int)SelectedHandleEnum.OUTSIDE_VALUES_120;
                case "outside >= < outside": return (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_LOWER_BOUND_121;
                case "outside > <= outside": return (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_122;
                case "outside >= <= outside": return (int)SelectedHandleEnum.OUTSIDE_OR_EQUAL_WITH_BOUNDS_123;

                default: 
                    // Log error or handle unknown string
                    this.proghmi.sendError(kutsuja+functionname," Unknown selected handle string: "+selectedhandlestring,-1113,4,4);
                    return (int)SelectedHandleEnum.ERROR_SELECTION_MINUS_1; // Virhekoodi -1 tuntemattomille merkkijonoille
            }
        }

        /// <summary>
        /// Suorittaa vertailuoperaation kahden BlockAtomValue:n välillä ja asettaa tuloksen.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <returns>{int} Palauttaa 1, jos toimenpide onnistui. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos toimenpide epäonnistui.</returns>
        public override int ExecuteBlock(string kutsuja, MotherConnectionRectangle motherconnrect, OneSlot oneslot=null)
        {
            string functionname = "->(CB)ExecuteBlock";
            try
            {
                // Hae A ja B arvot
                long handleUID_B = this.ReturnBlockHandlesRef.ReturnBlockHandleUIDFirst(kutsuja+functionname,(int)ConnectionRectangles.connectionBoxType.YELLOW_BOX_COMPARE_VALUE_1);
                    // Suorita between/outside vertailu
                long handleUID_B2 = this.ReturnBlockHandlesRef.ReturnBlockHandleUIDNext(kutsuja+functionname,(int)ConnectionRectangles.connectionBoxType.YELLOW_BOX_COMPARE_VALUE_1);                
                long handleUID_A = this.ReturnBlockHandlesRef.ReturnBlockHandleUIDFirst(kutsuja+functionname,(int)ConnectionRectangles.connectionBoxType.GREEN_BOX_CHECK_VALUE_2);

                if (handleUID_A < 0 || handleUID_B < 0)
                {
                    this.proghmi.sendError(kutsuja + functionname, "Invalid handle UID for A or B. HandleUID_A: " + handleUID_A + ", HandleUID_B: " + handleUID_B, -1353, 4, 4);
                    return -10;
                }

                BlockHandle handleA = this.ReturnBlockHandlesRef.ReturnBlockHandleByUID(kutsuja+functionname,handleUID_A);
                BlockHandle handleB = this.ReturnBlockHandlesRef.ReturnBlockHandleByUID(kutsuja+functionname,handleUID_B);

                if (handleA == null || handleB == null)
                {
                    this.proghmi.sendError(kutsuja + functionname, "Handle A or B is null. HandleUID_A: " + handleUID_A + ", HandleUID_B: " + handleUID_B, -1354, 4, 4);
                    return -2;
                }

                BlockAtomValue atomValueA = handleA.ReturnBlockAtomValueRef;
                BlockAtomValue atomValueB = handleB.ReturnBlockAtomValueRef;

                if (atomValueA == null || atomValueB == null)
                {
                    this.proghmi.sendError(kutsuja + functionname, "BlockAtomValue A or B is null. HandleUID_A: " + handleUID_A + ", HandleUID_B: " + handleUID_B, -1355, 4, 4);
                    return -3;
                }

                // Tarkista, että tyypit ovat samat
                if (atomValueA.AtomType != atomValueB.AtomType)
                {
                    this.proghmi.sendError(kutsuja + functionname, "BlockAtomValue types do not match. AtomType_A: " + atomValueA.AtomType + ", AtomType_B: " + atomValueB.AtomType, -1356, 4, 4);
                    return -4;
                }

                int comparisonResult = 0;
                if (motherconnrect==null) {
                    this.proghmi.sendError(kutsuja + functionname, "Given MotherConnectionRectangle was null!", -1363, 4, 4);
                    return -12;
                }
                int comparisonType= this.SwitchSelectedHandleStringToEnumInt(kutsuja+functionname,motherconnrect.StoredUIcomps.StoredParamValues.SelectedOperator);
                this.SelectedHandle=comparisonType;

                // Tarkista vertailutyyppi
                if (this.comparisonblocktype == (int)ComparisonBlockTypeEnum.NORMAL_COMPARISON_1 && comparisonType >= (int)ComparisonBlockTypeEnum.NORMAL_COMPARISON_MIN_INDEX_VALUE && comparisonType <= (int)ComparisonBlockTypeEnum.NORMAL_COMPARISON_MAX_INDEX_VALUE)
                {
                    comparisonResult = this.PerformNormalComparison(kutsuja+functionname,atomValueA, atomValueB, comparisonType);
                }
                else if (this.comparisonblocktype == (int)ComparisonBlockTypeEnum.BETWEEN_OUTSIDE_COMPARISON_2 && comparisonType >= (int)ComparisonBlockTypeEnum.BETWEEN_OUTSIDE_COMPARISON_MIN_INDEX_VALUE && comparisonType<= (int)ComparisonBlockTypeEnum.BETWEEN_OUTSIDE_COMPARISON_MAX_INDEX_VALUE)
                {

                    if (handleUID_B2 < 0)
                    {
                        this.proghmi.sendError(kutsuja + functionname, "Invalid handle UID for A2. HandleUID_B2: " + handleUID_B2, -1357, 4, 4);
                        return -5;
                    }

                    BlockHandle handleB2 = this.ReturnBlockHandlesRef.ReturnBlockHandleByUID(kutsuja+functionname,handleUID_B2);

                    if (handleB2 == null)
                    {
                        this.proghmi.sendError(kutsuja + functionname, "Handle A2 is null. HandleUID_B2: " + handleUID_B2, -1358, 4, 4);
                        return -6;
                    }

                    BlockAtomValue atomValueB2 = handleB2.ReturnBlockAtomValueRef;

                    if (atomValueB2 == null)
                    {
                        this.proghmi.sendError(kutsuja + functionname, "BlockAtomValue A2 is null. HandleUID_B2: " + handleUID_B2, -1359, 4, 4);
                        return -7;
                    }

                    if (atomValueA.AtomType != atomValueB2.AtomType)
                    {
                        this.proghmi.sendError(kutsuja + functionname, "BlockAtomValue types do not match between A and B2. AtomType_A: " + atomValueA.AtomType + ", AtomType_B2: " + atomValueB2.AtomType, -1360, 4, 4);
                        return -8;
                    }

                    comparisonResult = this.PerformBetweenOutsideComparison(kutsuja+functionname, atomValueB, atomValueA, atomValueB2, comparisonType);
                }
                else
                {
                    this.proghmi.sendError(kutsuja + functionname, "Invalid comparison type or block type. ComparisonType: " + comparisonType + ", BlockType: " + this.comparisonblocktype, -1361, 4, 4);
                    return -9;
                }

                // Aseta tulokset BlockAtomValue-objekteihin
                int res = comparisonResult == 1 ? 1 : 0;
                bool resbool = comparisonResult == 1 ? true : false;
                string resstr = comparisonResult == 1 ? "true" : "false";
                this.BlockResultValue.AtomMultiSet(kutsuja+functionname,(int)BlockAtomValue.AtomTypeEnum.LONG_AND_INT_AND_DEC_AND_STRING_AND_BOOL,res,res,(decimal)res,resstr,resbool);
                this.comptrueBlockResultValue.AtomMultiSet(kutsuja+functionname,(int)BlockAtomValue.AtomTypeEnum.LONG_AND_INT_AND_DEC_AND_STRING_AND_BOOL,res,res,(decimal)res,resstr,resbool);
                if (comparisonResult==1) {
                    this.comptrueBlockResultValue.ActiveSynapse=(int)BlockAtomValue.activeSynapseSendForward.SENDING_FORWARD_TRUE_1;
                } else {
                    this.comptrueBlockResultValue.ActiveSynapse=(int)BlockAtomValue.activeSynapseSendForward.SENDING_FORWARD_FALSE_0;
                }

                res = comparisonResult == 1 ? 0 : 1;
                resbool = comparisonResult == 1 ? false : true;
                resstr = comparisonResult == 1 ? "false" : "true";
                this.inverseBlockResultValue.AtomMultiSet(kutsuja+functionname,(int)BlockAtomValue.AtomTypeEnum.LONG_AND_INT_AND_DEC_AND_STRING_AND_BOOL,res,res,(decimal)res,resstr,resbool);
                if (comparisonResult==1) {
                    this.inverseBlockResultValue.ActiveSynapse=(int)BlockAtomValue.activeSynapseSendForward.SENDING_FORWARD_FALSE_0; 
                } else {
                    this.inverseBlockResultValue.ActiveSynapse=(int)BlockAtomValue.activeSynapseSendForward.SENDING_FORWARD_TRUE_1;
                }                

                // Hae kolmas kahva iterationclass=3 (laajennus)
                SortedList<long, BlockHandle> class3Handles = GetBlockHandlesByIterationClass(kutsuja + functionname, (int)ConnectionRectangles.connectionBoxType.RED_BOX_RESULT_VALUE_3);
                if (class3Handles!=null) {
                    int correctnumberofhandles=3;
                    if (class3Handles.Count != correctnumberofhandles) {
                        for (int i=0; i<correctnumberofhandles; i++) {
                            if (i==correctnumberofhandles-1) {
                                class3Handles.ElementAt(i).Values.ReturnBlockAtomValueRef.CopyFrom(this.inverseBlockResultValue); // Tämä, kun välitetään eteenpäin false arvo
                            } else if (i==0) {
                                class3Handles.ElementAt(i).Values.ReturnBlockAtomValueRef.CopyFrom(this.BlockResultValue); // Tämä, kun välitetään eteenpäin result arvo
                            } else {
                                class3Handles.ElementAt(i).Values.ReturnBlockAtomValueRef.CopyFrom(this.comptrueBlockResultValue); // Tämä, kun välitetään eteenpäin true arvo
                            }
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Insufficient amount of BlockHandle for comparison. Amount:"+class3Handles.Count, -1380, 4, 4);
                        return -13;
                    }
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Class three handles was null!", -1381, 4, 4);
                    return -14;                    
                }          

                // Palauta onnistuminen asetetun BlockAtomValuen tyyppi enumeraatio numerona
                return (int)BlockAtomValue.AtomTypeEnum.LONG_AND_INT_AND_DEC_AND_STRING_AND_BOOL;
            }
            catch (Exception ex)
            {
                this.proghmi.sendError(kutsuja + functionname, "Exception: " + ex.Message, -1362, 4, 4);
                return -11;
            }
        }

        /// <summary>
        /// Suorittaa normaalin vertailun kahden BlockAtomValue:n välillä.
        /// </summary>
        /// <param name="atomValueA">BlockAtomValue, jota verrataan.</param>
        /// <param name="atomValueB">BlockAtomValue, jota vasten verrataan.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformNormalComparison(string kutsuja, BlockAtomValue atomValueA, BlockAtomValue atomValueB, int comparisonType)
        {
            string functionname = "->(CB)PerformNormalComparison";
            switch (atomValueA.AtomType)
            {
                case (int)BlockAtomValue.AtomTypeEnum.Int:
                    return PerformIntComparison(kutsuja + functionname, atomValueA.IntAtom, atomValueB.IntAtom, atomValueA.IntAtomMinusDiff, atomValueA.IntAtomPlusDiff, comparisonType);
                case (int)BlockAtomValue.AtomTypeEnum.String:
                    return PerformStringComparison(kutsuja + functionname, atomValueA.StringAtom, atomValueB.StringAtom, comparisonType);
                case (int)BlockAtomValue.AtomTypeEnum.Long:
                    return PerformLongComparison(kutsuja + functionname, atomValueA.LongAtom, atomValueB.LongAtom, atomValueA.LongAtomMinusDiff, atomValueA.LongAtomPlusDiff, comparisonType);
                case (int)BlockAtomValue.AtomTypeEnum.Decimal:
                    return PerformDecimalComparison(kutsuja + functionname, atomValueA.DecAtom, atomValueB.DecAtom, atomValueA.DecAtomMinusDiff, atomValueA.DecAtomPlusDiff, comparisonType);
                case (int)BlockAtomValue.AtomTypeEnum.Bool:
                    return PerformBoolComparison(kutsuja + functionname, atomValueA.BoolAtom, atomValueB.BoolAtom, comparisonType);
                default:
                    throw new InvalidOperationException("Unsupported BlockAtomValue type for normal comparison: " + atomValueA.AtomType);
            }
        }

        /// <summary>
        /// Suorittaa vertailun kahden long-arvon välillä.
        /// </summary>
        /// <param name="valueA">Long-arvo, jota verrataan.</param>
        /// <param name="valueB">Long-arvo, jota vasten verrataan.</param>
        /// <param name="alloweddifferenceminus">Hyväksyttävä negatiivinen arvoerotus, jolloin kohde vielä katsotaan samaksi.</param>
        /// <param name="alloweddifferenceplus">Hyväksyttävä positiivinen arvoerotus, jolloin kohde vielä katsotaan samaksi.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformLongComparison(string kutsuja, long valueA, long valueB, long alloweddifferenceminus, long alloweddifferenceplus, int comparisonType)
        {
            string functionname = "->(CB)PerformLongComparison";
            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.EQUAL_10:
                    return this.EqualThan(kutsuja + functionname, valueA, valueB, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.NOT_EQUAL_11:
                    return this.NotEqualThan(kutsuja + functionname, valueA, valueB, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BELOW_14:
                    return this.LesserThan(kutsuja + functionname, valueA, valueB, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BELOW_OR_EQUAL_15:
                    return this.LesserThanOrEqual(kutsuja + functionname, valueA, valueB, alloweddifferenceplus);
                case (int)SelectedHandleEnum.ABOVE_12:
                    return this.GreaterThan(kutsuja + functionname, valueA, valueB, alloweddifferenceminus);
                case (int)SelectedHandleEnum.ABOVE_OR_EQUAL_13:
                    return this.GreaterThanOrEqual(kutsuja + functionname, valueA, valueB, alloweddifferenceminus);
                default:
                    throw new InvalidOperationException("Invalid comparison type for long comparison: " + comparisonType);
            }
        }

        /// <summary>
        /// Suorittaa vertailun kahden decimal-arvon välillä.
        /// </summary>
        /// <param name="valueA">Decimal-arvo, jota verrataan.</param>
        /// <param name="valueB">Decimal-arvo, jota vasten verrataan.</param>
        /// <param name="alloweddifferenceminus">Hyväksyttävä negatiivinen arvoerotus, jolloin kohde vielä katsotaan samaksi.</param>
        /// <param name="alloweddifferenceplus">Hyväksyttävä positiivinen arvoerotus, jolloin kohde vielä katsotaan samaksi.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformDecimalComparison(string kutsuja, decimal valueA, decimal valueB, decimal alloweddifferenceminus, decimal alloweddifferenceplus, int comparisonType)
        {
            string functionname = "->(CB)PerformDecimalComparison";
            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.EQUAL_10:
                    return this.EqualThan(kutsuja + functionname, valueA, valueB, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.NOT_EQUAL_11:
                    return this.NotEqualThan(kutsuja + functionname, valueA, valueB, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BELOW_14:
                    return this.LesserThan(kutsuja + functionname, valueA, valueB, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BELOW_OR_EQUAL_15:
                    return this.LesserThanOrEqual(kutsuja + functionname, valueA, valueB, alloweddifferenceplus);
                case (int)SelectedHandleEnum.ABOVE_12:
                    return this.GreaterThan(kutsuja + functionname, valueA, valueB, alloweddifferenceminus);
                case (int)SelectedHandleEnum.ABOVE_OR_EQUAL_13:
                    return this.GreaterThanOrEqual(kutsuja + functionname, valueA, valueB, alloweddifferenceminus);
                default:
                    throw new InvalidOperationException("Invalid comparison type for decimal comparison: " + comparisonType);
            }
        }

        /// <summary>
        /// Suorittaa vertailun kahden bool-arvon välillä.
        /// </summary>
        /// <param name="valueA">Bool-arvo, jota verrataan.</param>
        /// <param name="valueB">Bool-arvo, jota vasten verrataan.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformBoolComparison(string kutsuja, bool valueA, bool valueB, int comparisonType)
        {
            string functionname = "->(CB)PerformBoolComparison";
            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.EQUAL_10:
                    return this.EqualThan(kutsuja + functionname, valueA, valueB);
                case (int)SelectedHandleEnum.NOT_EQUAL_11:
                    return this.NotEqualThan(kutsuja + functionname, valueA, valueB);
                default:
                    throw new InvalidOperationException("Invalid comparison type for bool comparison: " + comparisonType);
            }
        }


        /// <summary>
        /// Suorittaa vertailun kahden int-arvon välillä.
        /// </summary>
        /// <param name="valueA">Int-arvo, jota verrataan.</param>
        /// <param name="valueB">Int-arvo, jota vasten verrataan.</param>
        /// <param name="alloweddifferenceminus">Hyväksyttävä negatiivinen arvoerotus, jolloin kohde vielä katsotaan samaksi.</param>
        /// <param name="alloweddifferenceplus">Hyväksyttävä positiivinen arvoerotus, jolloin kohde vielä katsotaan samaksi.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformIntComparison(string kutsuja, int valueA, int valueB, int alloweddifferenceminus, int alloweddifferenceplus, int comparisonType)
        {
            string functionname = "->(CB)PerformIntComparison";
            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.EQUAL_10:
                    return this.EqualThan(kutsuja+functionname, valueA, valueB, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.NOT_EQUAL_11:
                    return this.NotEqualThan(kutsuja+functionname, valueA, valueB, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BELOW_14:
                    return this.LesserThan(kutsuja+functionname, valueA, valueB, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BELOW_OR_EQUAL_15:
                    return this.LesserThanOrEqual(kutsuja+functionname, valueA, valueB, alloweddifferenceplus);
                case (int)SelectedHandleEnum.ABOVE_12:
                    return this.GreaterThan(kutsuja+functionname, valueA, valueB, alloweddifferenceminus);
                case (int)SelectedHandleEnum.ABOVE_OR_EQUAL_13:
                    return this.GreaterThanOrEqual(kutsuja+functionname, valueA, valueB, alloweddifferenceminus);
                default:
                    throw new InvalidOperationException("Invalid comparison type for int comparison: " + comparisonType);
            }
        }        

        /// <summary>
        /// Suorittaa vertailun kahden string-arvon välillä.
        /// </summary>
        /// <param name="kutsuja">String, kutsujan polku.</param>
        /// <param name="valueA">String-arvo, jota verrataan.</param>
        /// <param name="valueB">String-arvo, jota vasten verrataan.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformStringComparison(string kutsuja, string valueA, string valueB, int comparisonType)
        {
            string functionname = "->(CB)PerformStringComparison";
            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.EQUAL_10:
                    return this.EqualThan(kutsuja + functionname, valueA, valueB);
                case (int)SelectedHandleEnum.NOT_EQUAL_11:
                    return this.NotEqualThan(kutsuja + functionname, valueA, valueB);
                default:
                    throw new InvalidOperationException("Invalid comparison type for string comparison: " + comparisonType);
            }
        }

        /// <summary>
        /// Suorittaa Between/Outside vertailun.
        /// </summary>
        /// <param name="testValue">BlockAtomValue, jota verrataan.</param>
        /// <param name="rangeValue1">BlockAtomValue, vertailuvälin ensimmäinen arvo.</param>
        /// <param name="rangeValue2">BlockAtomValue, vertailuvälin toinen arvo.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformBetweenOutsideComparison(string kutsuja, BlockAtomValue testValue, BlockAtomValue rangeValue1, BlockAtomValue rangeValue2, int comparisonType)
        {
            string functionname = "->(CB)PerformBetweenOutsideComparison";
            switch (testValue.AtomType)
            {
                case (int)BlockAtomValue.AtomTypeEnum.Int:
                    return PerformIntBetweenOutsideComparison(kutsuja+functionname, testValue.IntAtom, rangeValue1.IntAtom, rangeValue2.IntAtom, rangeValue1.IntAtomMinusDiff, rangeValue1.IntAtomPlusDiff, comparisonType);
                case (int)BlockAtomValue.AtomTypeEnum.Long:
                    return PerformLongBetweenOutsideComparison(kutsuja+functionname, testValue.LongAtom, rangeValue1.LongAtom, rangeValue2.LongAtom, rangeValue1.LongAtomMinusDiff, rangeValue1.LongAtomPlusDiff, comparisonType);
                case (int)BlockAtomValue.AtomTypeEnum.Decimal:
                    return PerformDecimalBetweenOutsideComparison(kutsuja+functionname, testValue.DecAtom, rangeValue1.DecAtom, rangeValue2.DecAtom, rangeValue1.DecAtomMinusDiff, rangeValue1.DecAtomPlusDiff, comparisonType);
                case (int)BlockAtomValue.AtomTypeEnum.Bool:
                    return PerformBoolBetweenOutsideComparison(kutsuja+functionname, testValue.BoolAtom, rangeValue1.BoolAtom, rangeValue2.BoolAtom, comparisonType);
                default:
                    throw new InvalidOperationException("Unsupported BlockAtomValue type for Between/Outside comparison: " + testValue.AtomType);
            }
        }

        /// <summary>
        /// Suorittaa Between/Outside vertailun kahden int-arvon välillä.
        /// </summary>
        /// <param name="valuec">Int-arvo, jota verrataan.</param>
        /// <param name="range1">Int-arvo, vertailuvälin ensimmäinen arvo.</param>
        /// <param name="range2">Int-arvo, vertailuvälin toinen arvo.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformIntBetweenOutsideComparison(string kutsuja, int valuec, int range1, int range2, decimal alloweddifferenceminus, decimal alloweddifferenceplus, int comparisonType)
        {
            string functionname = "->(CB)PerformIntBetweenOutsideComparison";
            int lower = Math.Min(range1, range2);
            int upper = Math.Max(range1, range2);

            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.BETWEEN_VALUES_110:
                    return this.Between(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_LOWER_BOUND_111:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_112:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_OR_EQUAL_WITH_BOUNDS_113:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_120:
                    return this.Outside(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_LOWER_BOUND_121:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_122:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_OR_EQUAL_WITH_BOUNDS_123:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                default:
                    throw new InvalidOperationException("Invalid comparison type for int Between/Outside comparison: " + comparisonType);
            }
        }

        /// <summary>
        /// Suorittaa Between/Outside vertailun kahden long-arvon välillä.
        /// </summary>
        /// <param name="valuec">Long-arvo, jota verrataan.</param>
        /// <param name="range1">Long-arvo, vertailuvälin ensimmäinen arvo.</param>
        /// <param name="range2">Long-arvo, vertailuvälin toinen arvo.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformLongBetweenOutsideComparison(string kutsuja, long valuec, long range1, long range2, decimal alloweddifferenceminus, decimal alloweddifferenceplus, int comparisonType)
        {
            string functionname = "->(CB)PerformLongBetweenOutsideComparison";
            long lower = Math.Min(range1, range2);
            long upper = Math.Max(range1, range2);

            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.BETWEEN_VALUES_110:
                    return this.Between(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_LOWER_BOUND_111:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_112:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_OR_EQUAL_WITH_BOUNDS_113:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_120:
                    return this.Outside(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_LOWER_BOUND_121:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_122:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_OR_EQUAL_WITH_BOUNDS_123:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                default:
                    throw new InvalidOperationException("Invalid comparison type for long Between/Outside comparison: " + comparisonType);
            }
        }

        /// <summary>
        /// Suorittaa Between/Outside vertailun kahden decimal-arvon välillä.
        /// </summary>
        /// <param name="valuec">Decimal-arvo, jota verrataan.</param>
        /// <param name="range1">Decimal-arvo, vertailuvälin ensimmäinen arvo.</param>
        /// <param name="range2">Decimal-arvo, vertailuvälin toinen arvo.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformDecimalBetweenOutsideComparison(string kutsuja, decimal valuec, decimal range1, decimal range2, decimal alloweddifferenceminus, decimal alloweddifferenceplus, int comparisonType)
        {
            string functionname = "->(CB)PerformDecimalBetweenOutsideComparison";
            decimal lower = Math.Min(range1, range2);
            decimal upper = Math.Max(range1, range2);

            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.BETWEEN_VALUES_110:
                    return this.Between(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_LOWER_BOUND_111:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_112:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.BETWEEN_OR_EQUAL_WITH_BOUNDS_113:
                    return this.BetweenOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_120:
                    return this.Outside(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_LOWER_BOUND_121:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_122:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                case (int)SelectedHandleEnum.OUTSIDE_OR_EQUAL_WITH_BOUNDS_123:
                    return this.OutsideOrEqual(kutsuja + functionname, valuec, lower, upper, alloweddifferenceminus, alloweddifferenceplus);
                default:
                    throw new InvalidOperationException("Invalid comparison type for decimal Between/Outside comparison: " + comparisonType);
            }
        }

        /// <summary>
        /// Suorittaa Between/Outside vertailun kahden bool-arvon välillä.
        /// </summary>
        /// <param name="valuec">Bool-arvo, jota verrataan.</param>
        /// <param name="range1">Bool-arvo, vertailuvälin ensimmäinen arvo.</param>
        /// <param name="range2">Bool-arvo, vertailuvälin toinen arvo.</param>
        /// <param name="comparisonType">Vertailutyyppi.</param>
        /// <returns>Vertailun tulos: 1 jos tosi, 0 jos epätosi.</returns>
        private int PerformBoolBetweenOutsideComparison(string kutsuja, bool valuec, bool range1, bool range2, int comparisonType)
        {
            string functionname = "->(CB)PerformBoolBetweenOutsideComparison";
            bool lower = range1 && range2; // Both must be true for the range to be considered true
            bool upper = range1 || range2; // Either can be true for the range to be considered true

            switch (comparisonType)
            {
                case (int)SelectedHandleEnum.BETWEEN_VALUES_110:
                case (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_LOWER_BOUND_111:
                case (int)SelectedHandleEnum.BETWEEN_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_112:
                case (int)SelectedHandleEnum.BETWEEN_OR_EQUAL_WITH_BOUNDS_113:
                    return (valuec == lower && valuec == upper) ? 1 : 0;
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_120:
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_LOWER_BOUND_121:
                case (int)SelectedHandleEnum.OUTSIDE_VALUES_OR_EQUAL_WITH_HIGHER_BOUND_122:
                case (int)SelectedHandleEnum.OUTSIDE_OR_EQUAL_WITH_BOUNDS_123:
                    return (valuec != lower || valuec != upper) ? 1 : 0;
                default:
                    throw new InvalidOperationException("Invalid comparison type for bool Between/Outside comparison: " + comparisonType);
            }
        }



        /// <summary> Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle samat. Jos ero on pienempi kuin alloweddifference, niin kohteet vielä katsotaan samoiksi. </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="valuehandle"> decimal, syötettävän arvon kahva </param>
        /// <param name="comparisonhandle"> decimal, vertailuarvon kahva </param>
        /// <param name="alloweddifferencmin"> decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi </param>
        /// <param name="alloweddifferencmax"> decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus, jotta ne vielä katsotaan samaksi arvoksi </param>
        /// <returns> {int} palauttaa 0, jos arvot eivät ole samat ja 1, jos arvot ovat samat. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe </returns>
        public int EqualThan(string kutsuja, decimal valuehandle, decimal comparisonhandle, decimal alloweddifferenceminus=0, decimal alloweddifferenceplus=0)
        {
            int retVal=-1;
            if (alloweddifferenceminus==0 && alloweddifferenceplus==0) {
                if (valuehandle==comparisonhandle) {
                    retVal=1;
                } else {
                    retVal=0;
                }
            } else {
                if (valuehandle>=comparisonhandle-alloweddifferenceminus && valuehandle<=comparisonhandle+alloweddifferenceplus) {
                    retVal=1;
                } else {
                    retVal=0;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle samat. Jos ero on pienempi kuin alloweddifference, niin kohteet vielä katsotaan samoiksi.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Long, vertailuarvon kahva.</param>
        /// <param name="alloweddifferenceminus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <returns>Palauttaa 0, jos arvot eivät ole samat ja 1, jos arvot ovat samat. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int EqualThan(string caller, long valuehandle, long comparisonhandle, long alloweddifferenceminus=0, long alloweddifferenceplus=0)
        {
            int retVal=-1;
            // If no allowed difference, compare for equality
            if (alloweddifferenceminus == 0 && alloweddifferenceplus == 0)
            {
                if (valuehandle == comparisonhandle)
                {
                    retVal=1;
                }
                else
                {
                    retVal=0;
                }
            }
            // If allowed difference is specified, check for it
            else
            {
                if (valuehandle >= comparisonhandle - alloweddifferenceminus && valuehandle <= comparisonhandle + alloweddifferenceplus)
                {
                    retVal=1;
                } else {
                    retVal=0;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, ovatko valuehandle ja comparisonhandle samat.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">String, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">String, vertailuarvon kahva.</param>
        /// <returns>Palauttaa 0, jos arvot eivät ole samat ja 1, jos arvot ovat samat. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int EqualThan(string caller, string valuehandle, string comparisonhandle)
        {
            int retVal=-1;
            // Simply compare the string values for equality
            if (valuehandle == comparisonhandle)
            {
                retVal=1;
            }
            else
            {
                retVal=0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, ovatko valuehandle ja comparisonhandle samat.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Bool, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Bool, vertailuarvon kahva.</param>
        /// <returns>Palauttaa 0, jos arvot eivät ole samat ja 1, jos arvot ovat samat. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int EqualThan(string caller, bool valuehandle, bool comparisonhandle)
        {
            int retVal=-1;
            // Simply compare the string values for equality
            if (valuehandle == comparisonhandle)
            {
                retVal=1;
            }
            else
            {
                retVal=0;
            }
            return retVal;
        }        

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle erisuuruiset. Jos ero on suurempi kuin alloweddifference, niin kohteet katsotaan erisuuriksi.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Decimal, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Decimal, vertailuarvon kahva.</param>
        /// <param name="alloweddifferenceminus">Decimal, minimi määrä, jonka verran arvot saavat olla suuremmat toisistaan miinus suuntaan, jotta ne katsotaan erisuuriksi.</param>
        /// <param name="alloweddifferenceplus">Decimal, minimi määrä, jonka verran arvot saavat olla suuremmat toisistaan plus suuntaan, jotta ne katsotaan erisuuriksi.</param>
        /// <returns> {int} Palauttaa 0, jos arvot ovat samat ja 1, jos arvot ovat erisuuruiset. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int NotEqualThan(string caller, decimal valuehandle, decimal comparisonhandle, decimal alloweddifferenceminus=0, decimal alloweddifferenceplus=0)
        {
            int retVal=-1;
            // If no allowed difference, compare for equality
            if (alloweddifferenceminus == 0 && alloweddifferenceplus == 0)
            {
                if (valuehandle != comparisonhandle) // Check if not equal
                {
                    retVal=1;
                }
                else
                {
                    retVal=0;
                }
            }
            // If allowed difference is specified, check for it
            else
            {
                if (valuehandle < comparisonhandle - alloweddifferenceminus || valuehandle > comparisonhandle + alloweddifferenceplus) // Check if out of range
                {
                    retVal=1;
                }
                else
                {
                    retVal=0;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle erisuuruiset.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Long, vertailuarvon kahva.</param>
        /// <param name="alloweddifferenceminus">Decimal, minimi määrä, jonka verran arvot saavat olla suuremmat toisistaan miinus suuntaan, jotta ne katsotaan erisuuriksi.</param>
        /// <param name="alloweddifferenceplus">Decimal, minimi määrä, jonka verran arvot saavat olla suuremmat toisistaan plus suuntaan, jotta ne katsotaan erisuuriksi.</param>
        /// <returns>Palauttaa 0, jos arvot ovat samat ja 1, jos arvot ovat erisuuruiset. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int NotEqualThan(string caller, long valuehandle, long comparisonhandle, long alloweddifferenceminus=0, long alloweddifferenceplus=0)
        {
            int retVal=-1;
            if (alloweddifferenceminus == 0 && alloweddifferenceplus == 0) {
                if (valuehandle != comparisonhandle) // Check if not equal
                {
                    retVal=1;
                }
                else
                {
                    retVal=0;
                }
            } else {
                if (valuehandle < comparisonhandle - alloweddifferenceminus || valuehandle > comparisonhandle + alloweddifferenceplus) // Check if out of range
                {
                    retVal=1;
                }
                else
                {
                    retVal=0;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle erisuuruiset.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">String, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">String, vertailuarvon kahva.</param>
        /// <returns>Palauttaa 0, jos arvot ovat samat ja 1, jos arvot ovat erisuuruiset. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int NotEqualThan(string caller, string valuehandle, string comparisonhandle)
        {
            int retVal=-1;
            if (valuehandle != comparisonhandle) // Check if not equal
            {
                retVal=1;
            }
            else
            {
                retVal=0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle erisuuruiset.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Bool, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Bool, vertailuarvon kahva.</param>
        /// <returns>Palauttaa 0, jos arvot ovat samat ja 1, jos arvot ovat erisuuruiset. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int NotEqualThan(string caller, bool valuehandle, bool comparisonhandle)
        {
            int retVal=-1;
            if (valuehandle != comparisonhandle) // Check if not equal
            {
                retVal=1;
            }
            else
            {
                retVal=0;
            }
            return retVal;
        }        
        
        /// <summary> 
        /// Tämä funktio tarkastaa, onko valuehandle suurempi tai yhtäsuuri kuin comparisonhandle. 
        /// Jos ero on pienempi kuin alloweddifferenceminus, niin kohteet vielä katsotaan samoiksi. 
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="valuehandle"> decimal, syötettävän arvon kahva </param>
        /// <param name="comparisonhandle"> decimal, vertailuarvon kahva </param>
        /// <param name="alloweddifferenceminus"> decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi </param>
        /// <returns> {int} palauttaa 0, jos valuehandle on pienempi kuin comparisonhandle ja 1, jos valuehandle on suurempi tai yhtäsuuri kuin comparisonhandle. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe </returns>
        public int GreaterThanOrEqual(string kutsuja, decimal valuehandle, decimal comparisonhandle, decimal alloweddifferenceminus=0)
        {
            int retVal=-1;
            if (valuehandle >= comparisonhandle - alloweddifferenceminus) {
                retVal=1;
            } else {
                retVal=0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle suurempi tai yhtäsuuri kuin comparisonhandle. 
        /// Jos ero on pienempi kuin alloweddifferenceminus, niin kohteet vielä katsotaan samoiksi.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Long, vertailuarvon kahva.</param>
        /// <param name="alloweddifferenceminus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <returns>Palauttaa 0, jos valuehandle on pienempi kuin comparisonhandle ja 1, jos valuehandle on suurempi tai yhtäsuuri kuin comparisonhandle. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int GreaterThanOrEqual(string caller, long valuehandle, long comparisonhandle, long alloweddifferenceminus=0)
        {
            int retVal=-1;
            if (valuehandle >= comparisonhandle - alloweddifferenceminus) {
                retVal=1;
            } else {
                retVal=0;
            }
            return retVal;
        }

        /// <summary> 
        /// Tämä funktio tarkastaa, onko valuehandle pienempi tai yhtäsuuri kuin comparisonhandle. 
        /// Jos ero on pienempi kuin alloweddifferenceplus, niin kohteet vielä katsotaan samiksi. 
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="valuehandle"> decimal, syötettävän arvon kahva </param>
        /// <param name="comparisonhandle"> decimal, vertailuarvon kahva </param>
        /// <param name="alloweddifferenceplus"> decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi </param>
        /// <returns> {int} palauttaa 0, jos valuehandle on suurempi kuin comparisonhandle ja 1, jos valuehandle on pienempi tai yhtäsuuri kuin comparisonhandle. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe </returns>
        public int LesserThanOrEqual(string kutsuja, decimal valuehandle, decimal comparisonhandle, decimal alloweddifferenceplus=0)
        {
            int retVal=-1;
            if (valuehandle <= comparisonhandle + alloweddifferenceplus) {
                retVal=1;
            } else {
                retVal=0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle pienempi tai yhtäsuuri kuin comparisonhandle. 
        /// Jos ero on pienempi kuin alloweddifferenceplus, niin kohteet vielä katsotaan samiksi.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Long, vertailuarvon kahva.</param>
        /// <param name="alloweddifferenceplus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <returns>Palauttaa 0, jos valuehandle on suurempi kuin comparisonhandle ja 1, jos valuehandle on pienempi tai yhtäsuuri kuin comparisonhandle. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int LesserThanOrEqual(string caller, long valuehandle, long comparisonhandle, long alloweddifferenceplus=0)
        {
            int retVal=-1;
            if (valuehandle <= comparisonhandle + alloweddifferenceplus) {
                retVal=1;
            } else {
                retVal=0;
            }
            return retVal;
        }

        /// <summary> 
        /// Tämä funktio tarkastaa, onko valuehandle suurempi kuin comparisonhandle. 
        /// Jos ero on pienempi kuin alloweddifferenceminus, niin kohteet vielä katsotaan samoiksi. 
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="valuehandle"> decimal, syötettävän arvon kahva </param>
        /// <param name="comparisonhandle"> decimal, vertailuarvon kahva </param>
        /// <param name="alloweddifferenceminus"> decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi </param>
        /// <returns> {int} palauttaa 0, jos valuehandle on pienempi kuin comparisonhandle ja 1, jos valuehandle on suurempi tai yhtäsuuri kuin comparisonhandle. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe </returns>
        public int GreaterThan(string kutsuja, decimal valuehandle, decimal comparisonhandle, decimal alloweddifferenceminus=0)
        {
            int retVal=-1;
            if (valuehandle > comparisonhandle - alloweddifferenceminus) {
                retVal=1;
            } else {
                retVal=0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle suurempi kuin comparisonhandle. 
        /// Jos ero on pienempi kuin alloweddifferenceminus, niin kohteet vielä katsotaan samoiksi.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Long, vertailuarvon kahva.</param>
        /// <param name="alloweddifferenceminus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <returns>Palauttaa 0, jos valuehandle on pienempi kuin comparisonhandle ja 1, jos valuehandle on suurempi tai yhtäsuuri kuin comparisonhandle. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int GreaterThan(string caller, long valuehandle, long comparisonhandle, long alloweddifferenceminus=0)
        {
            int retVal=-1;
            if (valuehandle > comparisonhandle - alloweddifferenceminus) {
                retVal=1;
            } else {
                retVal=0;
            }
            return retVal;
        }

        /// <summary> 
        /// Tämä funktio tarkastaa, onko valuehandle pienempi kuin comparisonhandle. 
        /// Jos ero on pienempi kuin alloweddifferenceplus, niin kohteet vielä katsotaan samoiksi. 
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="valuehandle"> decimal, syötettävän arvon kahva </param>
        /// <param name="comparisonhandle"> decimal, vertailuarvon kahva </param>
        /// <param name="alloweddifferenceplus"> decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi </param>
        /// <returns> {int} palauttaa 0, jos valuehandle on suurempi kuin comparisonhandle ja 1, jos valuehandle on pienempi tai yhtäsuuri kuin comparisonhandle. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe </returns>
        public int LesserThan(string kutsuja, decimal valuehandle, decimal comparisonhandle, decimal alloweddifferenceplus=0)
        {
            int retVal=-1;
            if (valuehandle < comparisonhandle + alloweddifferenceplus) {
                retVal=1;
            } else {
                retVal=0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle pienempi kuin comparisonhandle. 
        /// Jos ero on pienempi kuin alloweddifferenceplus, niin kohteet vielä katsotaan samiksi.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandle">Long, vertailuarvon kahva.</param>
        /// <param name="alloweddifferenceplus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <returns>Palauttaa 0, jos valuehandle on suurempi kuin comparisonhandle ja 1, jos valuehandle on pienempi tai yhtäsuuri kuin comparisonhandle. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int LesserThan(string caller, long valuehandle, long comparisonhandle, long alloweddifferenceplus=0)
        {
            int retVal=-1;
            if (valuehandle < comparisonhandle + alloweddifferenceplus) {
                retVal=1;
            } else {
                retVal=0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle kahvojen välissä tai yhtäsuuri kuin toinen comparisonhadle kahvoista.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandlelower">Long, vertailuarvon kahvoista pienempi</param>
        /// <param name="comparisonhandlehigher">Long, vertailuarvon kahvoista suurempi</param>
        /// <param name="alloweddifferenceminus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param> 
        /// <returns>Palauttaa 1, jos valuehandle on comparisonhandle kahvojen välissä, muuten 0. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int BetweenOrEqual(string caller, long valuehandle, long comparisonhandlelower, long comparisonhandlehigher, long alloweddifferenceminus=0, long alloweddifferenceplus=0)
        {
            int retVal=-1;

            if (valuehandle <= comparisonhandlehigher+alloweddifferenceplus && valuehandle >= comparisonhandlelower-alloweddifferenceminus) // Check if out of range
            {
                retVal=1;
            }
            else
            {
                retVal=0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle kahvojen välissä.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandlelower">Long, vertailuarvon kahvoista pienempi.</param>
        /// <param name="comparisonhandlehigher">Long, vertailuarvon kahvoista suurempi.</param>
        /// <param name="alloweddifferenceminus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <returns>Palauttaa 1, jos valuehandle on comparisonhandle kahvojen välissä, muuten 0. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int Between(string caller, long valuehandle, long comparisonhandlelower, long comparisonhandlehigher, long alloweddifferenceminus = 0, long alloweddifferenceplus = 0)
        {
            int retVal = -1;

            if (valuehandle < comparisonhandlehigher + alloweddifferenceplus && valuehandle > comparisonhandlelower - alloweddifferenceminus) // Check if out of range
            {
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle kahvojen ulkopuolella.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandlelower">Long, vertailuarvon kahvoista pienempi.</param>
        /// <param name="comparisonhandlehigher">Long, vertailuarvon kahvoista suurempi.</param>
        /// <param name="alloweddifferenceminus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <returns>Palauttaa 1, jos valuehandle on comparisonhandle kahvojen ulkopuolella, muuten 0. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int Outside(string caller, long valuehandle, long comparisonhandlelower, long comparisonhandlehigher, long alloweddifferenceminus = 0, long alloweddifferenceplus = 0)
        {
            int retVal = -1;

            if (valuehandle > comparisonhandlehigher + alloweddifferenceplus || valuehandle < comparisonhandlelower - alloweddifferenceminus) // Check if out of range
            {
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle kahvojen ulkopuolella tai yhtäsuuri kuin toinen comparisonhandle kahvoista.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Long, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandlelower">Long, vertailuarvon kahvoista pienempi.</param>
        /// <param name="comparisonhandlehigher">Long, vertailuarvon kahvoista suurempi.</param>
        /// <param name="alloweddifferenceminus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Long, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <returns>Palauttaa 1, jos valuehandle on comparisonhandle kahvojen ulkopuolella tai sama arvo, muuten 0. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int OutsideOrEqual(string caller, long valuehandle, long comparisonhandlelower, long comparisonhandlehigher, long alloweddifferenceminus = 0, long alloweddifferenceplus = 0)
        {
            int retVal = -1;

            if (valuehandle >= comparisonhandlehigher + alloweddifferenceplus || valuehandle <= comparisonhandlelower - alloweddifferenceminus) // Check if out of range
            {
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle kahvojen välissä tai yhtäsuuri kuin toinen comparisonhadle kahvoista.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Decimal, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandlelower">Decimal, vertailuarvon kahvoista pienempi.</param>
        /// <param name="comparisonhandlehigher">Decimal, vertailuarvon kahvoista suurempi.</param>
        /// <param name="alloweddifferenceminus">Decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param> 
        /// <returns>Palauttaa 1, jos valuehandle on comparisonhandle kahvojen välissä, muuten 0. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int BetweenOrEqual(string caller, decimal valuehandle, decimal comparisonhandlelower, decimal comparisonhandlehigher, decimal alloweddifferenceminus = 0, decimal alloweddifferenceplus = 0)
        {
            int retVal = -1;

            if (valuehandle <= comparisonhandlehigher + alloweddifferenceplus && valuehandle >= comparisonhandlelower - alloweddifferenceminus) // Check if out of range
            {
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle kahvojen välissä.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Decimal, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandlelower">Decimal, vertailuarvon kahvoista pienempi.</param>
        /// <param name="comparisonhandlehigher">Decimal, vertailuarvon kahvoista suurempi.</param>
        /// <param name="alloweddifferenceminus">Decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param> 
        /// <returns>Palauttaa 1, jos valuehandle on comparisonhandle kahvojen välissä, muuten 0. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int Between(string caller, decimal valuehandle, decimal comparisonhandlelower, decimal comparisonhandlehigher, decimal alloweddifferenceminus = 0, decimal alloweddifferenceplus = 0)
        {
            int retVal = -1;

            if (valuehandle < comparisonhandlehigher + alloweddifferenceplus && valuehandle > comparisonhandlelower - alloweddifferenceminus) // Check if out of range
            {
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle kahvojen ulkopuolella.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Decimal, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandlelower">Decimal, vertailuarvon kahvoista pienempi.</param>
        /// <param name="comparisonhandlehigher">Decimal, vertailuarvon kahvoista suurempi.</param>
        /// <param name="alloweddifferenceminus">Decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param> 
        /// <returns>Palauttaa 1, jos valuehandle on comparisonhandle kahvojen ulkopuolella, muuten 0. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int Outside(string caller, decimal valuehandle, decimal comparisonhandlelower, decimal comparisonhandlehigher, decimal alloweddifferenceminus = 0, decimal alloweddifferenceplus = 0)
        {
            int retVal = -1;

            if (valuehandle > comparisonhandlehigher + alloweddifferenceplus || valuehandle < comparisonhandlelower - alloweddifferenceminus) // Check if out of range
            {
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä funktio tarkastaa, onko valuehandle ja comparisonhandle kahvojen ulkopuolella tai yhtäsuuri kuin toinen comparisonhandle kahvoista.
        /// </summary>
        /// <param name="caller">String, kutsujan polku, joka kutsuu tätä kyseistä funktiota.</param>
        /// <param name="valuehandle">Decimal, syötettävän arvon kahva.</param>
        /// <param name="comparisonhandlelower">Decimal, vertailuarvon kahvoista pienempi.</param>
        /// <param name="comparisonhandlehigher">Decimal, vertailuarvon kahvoista suurempi.</param>
        /// <param name="alloweddifferenceminus">Decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan miinus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param>
        /// <param name="alloweddifferenceplus">Decimal, maksimi määrä, jonka verran arvot saavat poiketa toisistaan plus suuntaan, jotta ne vielä katsotaan samaksi arvoksi.</param> 
        /// <returns>Palauttaa 1, jos valuehandle on comparisonhandle kahvojen ulkopuolella tai yhtäsuuri, muuten 0. Jos pienempi kuin 0 palautuu, niin kyseessä on virhe.</returns>
        public int OutsideOrEqual(string caller, decimal valuehandle, decimal comparisonhandlelower, decimal comparisonhandlehigher, decimal alloweddifferenceminus = 0, decimal alloweddifferenceplus = 0)
        {
            int retVal = -1;

            if (valuehandle >= comparisonhandlehigher + alloweddifferenceplus || valuehandle <= comparisonhandlelower - alloweddifferenceminus) // Check if out of range
            {
                retVal = 1;
            }
            else
            {
                retVal = 0;
            }
            return retVal;
        }

    }
