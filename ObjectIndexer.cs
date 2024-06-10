    
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic; // List toiminto löytyy tämän sisästä
using System.Linq;
using System.CodeDom;

    /// <summary>
    /// Tämä luokka sisältää syntyneen virheen keskeisimmät tiedot
    /// </summary>
    public class ObjectRetrievalError
    {
        /// <summary>
        /// Yksilöllinen virhekoodi, esim. -1184 tms.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Koko virhe viesti samanmuotoisena, kuin mitä se esiintyy sendError käskyssä
        /// </summary>
        public string WholeErrorMessage { get; set; }

        /// <summary>
        /// Constructor - Tämä luokka sisältää syntyneen virheen keskeisimmät tiedot
        /// </summary>
        /// <param name="errorCode">Yksilöllinen virhekoodi, esim. -1184 tms.</param>
        /// <param name="wholeerrorMessage">Koko virhe viesti samanmuotoisena, kuin mitä se esiintyy sendError käskyssä</param>
        /// <returns> {void} </returns>
        public ObjectRetrievalError(int errorCode, string wholeerrorMessage)
        {
            ErrorCode = errorCode;
            WholeErrorMessage = wholeerrorMessage;
        }
    }

    /// <summary> Tämä luokka ottaa ylös säilytettäväkseen objektien yksilölliset id:t ja jakaa yksilöllisiä id numeroita ulospäin kysyttäessä </summary>
    public class ObjectIndexer
    {
        /// <summary>
        /// Viimeisimmän virheen tiedot, silloin kun on käytetty GetTypedObject metodia kohteen palauttamiseen
        /// </summary>
        private ObjectRetrievalError lastError;

        /// <summary>
        /// Gets the last error that occurred during the retrieval process.
        /// </summary>
        /// <returns>The last error object or null if no errors have occurred.</returns>
        public ObjectRetrievalError GetLastError {
            get { return lastError; }
        }         

        private long uniqueidnumberforobject;

        /// <summary> Lista kaikista kohteista, joille on myönnetty uniqidnum ja jotka on rekisteröity objectlist listalle </summary>
        public SortedList<long, ObjectIndexerField> objectlist;

        /// <summary> Palauttaa tämän hetkisen uniikin id:n arvon </summary>
        public long UniqueIdNumber { 
            get { return this.uniqueidnumberforobject; }
        }
        
        /// <summary> Antaa aina uuden uniikin id arvon </summary>
        public long GetNewUniqueIdNumber { 
            get { 
                this.uniqueidnumberforobject++; // Antaa aina uuden id numeron
                return this.uniqueidnumberforobject; 
            } 
        }

        /// <summary> Settaa uuden uniikin id referenssiluvun arvon siten, että vain kasvattaa jo olemassa olevaa lukua, eikä hyväksy pienempiä lukuja </summary>
        public long SetNewHighUniqueIdNumber { 
            set {
                if (value>this.uniqueidnumberforobject) {
                    this.uniqueidnumberforobject=value;
                }
            }
        }

        /// <summary>
        /// Tämä enumeraatio määrittelee, minkä tyyppisesti tämä Objektien indeksoija toimii. 
        /// Tämä tyyppi määrittää, käyttääkö sisäinen objectlist tunnuksinaan PermanentUID tyyppisiä avaimia (key) vai nollautuvaa juoksevan laskurin tyyppisiä avaimia. 
        /// Molemmat avaimet ovat long tyyppisiä lukuja, mutta PermanentUID sitoo kellon ajan mukaan, joka tekee UID numeroista yksilöllisiä, ellei ohjelma kaadu ja samalla minuutilla ohjelmaa käynnistetä uudestaan ja syystä tai toisesta ohjelman käynnistysluku UID
        /// tallennusmetodiikka ei toimi halutulla tavalla, joka periaatteessa huonolla tuurilla mahdollistaisi kahden samanlaisen UID numeron luomisen.
        /// </summary>
        public enum TypeOfObjectIndexer {
            /// <summary>
            /// Tyypillä 0 ObjectIndexer joka kerta, kun järjestelmä käynnistyy, nollautuu UID laskuri (joka luo aina seuraavan UID luvun uudelle kohteelle) (periaatteessa nollautuu). UID laskuri kuitenkin tietyin väliajoin tallentaa indeksilukunsa ja uusinta käynnistys
            /// lataa edellisen talletetun tiedon, lisää siihen suuren kiinteän luvun ja jatkaa kyseisestä indeksistä eteenpäin uusien UID lukujen antamista. Tämä ei kuitenkaan ole aukoton tapa antaa yksilöllisiä UID lukuja ja joskus UID generaattori saattaa aloittaa alusta, 
            /// jolloin voi tulla useita samalla UID numerolla luotuja kohteita, joita ei ohjelmassa sallita
            /// </summary>
            RUNNING_INSTANCE_RESETING_INDEXER_0=0,
            /// <summary>
            /// Tyypillä 1 ObjectIndexer luo uudet kohteet käyttäen PermanentUID tyyppiä, joka voidaan luoda PermanentUIDGenerator luokan käskyllä. Tällöin jokaiselle minuutille on mahdollista luoda 100 000 000 000 (tarkista minutemultiplier) määrän verran uusia UID lukuja, 
            /// joten jokainen UID on hyvin suurella todennäköisyydellä aina yksilöllinen.
            /// </summary>
            PERMANENT_UID_INDEXER_1=1
        };

        /// <summary>
        /// int, tämän indekserin tyyppi. Katso mahdolliset tyypit typeOfObjectIndexer enumeraatiosta ja selitykset, mitä mikäkin tyyppi tarkoittaa
        /// </summary>
        private int typeofobjindexer=0;

        /// <summary>
        /// Käyttöliittymän referenssi
        /// </summary> 
        private ProgramHMI proghmi;

        /// <summary> Constructor luokalle, joka jakaa uniikit id numerot eri objekteille ja tallentaa perustiedot kaikista rekisteröidyistä objekteista objectlist muuttujaan uniqidref:in perusteella </summary>
        /// <param name="typeofindexer"> int, tämän indekserin tyyppi. Katso mahdolliset tyypit typeOfObjectIndexer enumeraatiosta ja selitykset, mitä mikäkin tyyppi tarkoittaa </param>
        /// <param name="prohmi">ProgramHMI, käyttöliittymän referenssi | ProgramHMI, reference to the user interface</param>
        /// <returns> {void} </returns>
        public ObjectIndexer(int typeofindexer, ProgramHMI prohmi)
        {
            this.uniqueidnumberforobject=-1; // Aloitetaan -1:stä, jotta kun kysytään ensimmäisen kerran uniikia arvoa, niin se on tällöin 0
            this.typeofobjindexer=typeofindexer;
            this.proghmi=prohmi;
            this.objectlist = new SortedList<long, ObjectIndexerField>();
        }

        /// <summary> Tämä enumeration pitää sisällään eri tyyppiset objektien tyypit, joita tähän objectlist indeksiin voidaan tallentaa </summary>
        public enum indexerObjectTypes {
            /// <summary> Objekti on slottilistan slotti </summary>
            SLOTLIST_SLOT_0=0,
            /// <summary> Objekti on slottilistaan liittyvä assetti (AssetSilo luokka) </summary>
            ASSET_FOR_SLOTLIST_SLOT_1=1,
            /// <summary>
            /// Tämä objekti luo objectindexerin ja sitä voidaan pitää jossain määrin koko ohjelman ensimmäisenä/perus objektina (äitiobjekti)
            /// </summary>
            PRIMARY_CODE_OBJECT_2=2,
            /// <summary>
            /// SmartBot luokan objekti, joita voi olla periaatteessa ohjelman alla useita toiminnassa  yhtäaikaisesti (ei vielä luotu ominaisuus)
            /// </summary>
            SMARTBOT_OBJECT_3=3,
            /// <summary>
            /// Kun kohta 0 on objekti yhdelle slottilistan slotille, niin tämä on koko slottilistan objekti, josta löytyvät kaikki slotit indeksoituna
            /// </summary> 
            SLOTLIST_OBJECT_4=4,
            ASSET_DATA_SILO_OBJECT_5=5,
            
            /// <summary> Objekti on laatikko, joka voidaan yhdistää viivalla toiseen laatikkoon </summary>
            CONNECTION_RECTANGLE_100=100,
            /// <summary> Objekti on pikkulaatikkojen tietoja pitävä objektityyppi </summary>
            CONNECTION_RECTANGLES_101=101,
            /// <summary> Objekti on päätason laatikko, jonka alla on pikkulaatikoita </summary>
            MOTHER_COMPONENT_RECTANGLE_102=102,
            /// <summary> Graafiset komponentit, jotka on rekisteröity StoredUIObjectsForActionCentre luokkaan </summary>
            MOTHER_COMPONENT_STOREDUICOMPONENT_103=103,
            /// <summary>
            /// Objekti on lista tyyppiä long, ConnectionRectangle, joka kykenee käymään jokaisen kohteen yksi kerrallaan läpi ja palauttamaan aliobjektien UID tietoja
            /// </summary>
            CONNECTION_RECTANGLE_LIST_104=104,
            /// <summary> Objekti on viiva, joka yhdistää kaksi pikkulaatikkoa </summary>
            CONNECTION_LINE_110=110,
            /// <summary> Objekti on teksti, joka kirjoitetaan yhdysviivan yläpuolelle </summary>
            CONNECTION_LINE_TEXT_111=111,
            /// <summary>
            /// Objekti on ConnectionUIComponents luokan instanssi, joka sisältää kaikki yhdistämisviivaa koskevat graafiset komponentit
            /// </summary>
            CONNECTION_LINE_UI_COMPONENTS_112=112,
            /// <summary> StoredUIObjectsForActionCentre luokan parametri arvot, jotka on eriytetty StoredUIComponentParamValues luokkaan </summary>
            STOREDUI_PARAM_VALUES_150=150,
            /// <summary>
            /// ConnectionRectangle luokalle parametrien säilytyskontaineri luokan ConnectionRectangleData instanssi
            /// </summary>
            CONNECTION_RECTANGLE_UI_COMPONENTS_151=151,
            /// <summary> Objekti on käyttöliittymä komponentti tyypiltään textblock </summary>
            UI_COMPONENT_TEXTBLOCK_200=200,
            /// <summary> Objekti on käyttöliittymä komponentti tyypiltään textbox </summary>
            UI_COMPONENT_TEXTBOX_201=201,
            /// <summary> Objekti on käyttöliittymä komponentti tyypiltään combobox </summary>
            UI_COMPONENT_COMBOBOX_202=202,
            /// <summary> Koko ActionCentreUI luokan instanssi </summary> 
            ACTIONCENTREUI_CLASS_INSTANCE_300=300,
            /// <summary> Luodun ActionCentreUI popup ikkunan instanssi </summary>
            ACTIONCENTREUI_POPUP_WINDOW_301=301,
            /// <summary>
            /// ActionCentreConstructionHandler luokan instanssi, joka sijaitsee ActionCentreUI:n alla ja pitää sisällään tietomalliin liittyvät Rectangle listat sekä ConnectionHandlerin referenssin
            /// </summary>
            ACTIONCENTRE_CONSTRUCTION_HANDLER_320=320,
            /// <summary>
            /// IncomingHandleStatus objekti, joka säilyttää Connection UID:t, jotka eivät ole vielä saaneet arvoa
            /// </summary>
            ACTIONCENTRE_INCOMING_HANDLE_STATUS_339=339,
            /// <summary>
            /// ActionCentre:n CreateBlock käskyllä luotava ComparisonObject tyyppinen laatikko
            /// </summary>
            ACTIONCENTRE_COMPARISON_OBJECT_340=340,
            /// <summary>
            /// ActionCentre:n CreateBlock käskyllä luotava OperationBlockObject tyyppinen laatikko
            /// </summary>
            ACTIONCENTRE_OPERATIONBLOCK_OBJECT_341=341,
            /// <summary>
            /// BlockHandles-instanssi, joka vastaa kahvojen tietojen ylläpidosta
            /// </summary> 
            ACTIONCENTRE_OPERATIONBLOCK_BLOCKHANDLES_400=400,
            /// <summary>
            /// BlockHandle instanssi, joka kuuluu yhtenä Blockhandles luokan alle sen ylläpitämään UID perusteiseen listaan
            /// </summary>
            ACTIONCENTRE_OPERATIONBLOCK_BLOCKHANDLE_401=401,
            /// <summary> Luodun AttemptObject objektin instanssi </summary>
            ATTEMPT_OBJECT_1500=1500,
            /// <summary> Luodun AttemptAndInstructionObject luokan instanssi </summary>
            ATTEMPT_AND_INSTRUCTION_OBJECT_1550=1550,
            /// <summary> Luodun StepEngine luokan instanssi </summary>
            STEP_ENGINE_OBJECT_2000=2000,
            /// <summary> StepEngine luokan alle luodun StepEngineSuperBlock luokan instanssi </summary>
            STEP_ENGINE_SUPER_BLOCK_2020=2020,
            /// <summary> StepEngineSuperBlock luokan alle luodun StepEngineInstructions luokan instanssi </summary>
            STEP_ENGINE_INSTRUCTION_2040=2040,
            /// <summary>
            /// UID WPF tyyppiseen käyttöliittymän pääikkunaan, josta ohjelman suoritus alkaa kyseisellä käyttöliittymällä
            /// </summary>
            PROGRAM_MAIN_WINDOW_UITYPE_TWO_2100=2100
        }

        /// <summary> Tämä funktio lisää object indexeriin yksilöllisen objektin tiedot varuilta talteen ja palauttaa käyttäjälle uuden yksilöllisen numeron talletettavaksi objektille </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="slotvalue"> decimal, slotin arvo, jonka alueella myynti tapahtui </param>
        /// <param name="objtype"> int, 0 = slotti, 1 = assetti, 100=ConnectionRectangle, 101=ConnectionRectangles, 102=MotherConnectionRectangle, 110=Connection jne. </param>
        /// <param name="objarrayind"> int, arraylist index tai assetin tapauksessa (0=sellersiloobj, 1=holdingsiloobj) </param>
        /// <returns> {long}, palauttaa objektin uniikin yksilöllisen numeron, jolla objektia voidaan etsiä. Jos pienempi kuin 0, niin kyseessä virhekoodi ja koodi kertoo, minkälainen virhe oli kyseessä </returns>
        /*
        public long AddObjectToIndexer(string kutsuja, decimal slotvalue, int objtype, int objarrayind)
        {
            long retVal;
            long permVal;
            string functionname="->(OI)AddObjectToIndexer#1";

            retVal=this.GetNewUniqueIdNumber;
            permVal=PermanentUIDGenerator.ReturnPermanentUID(kutsuja+functionname,retVal);
            // Luodaan objectindexer kohteet vanhalla tavalla
            if (this.typeofobjindexer==(int)TypeOfObjectIndexer.RUNNING_INSTANCE_RESETING_INDEXER_0) {
                this.objectlist.Add(retVal,new ObjectIndexerField(objtype,objarrayind));
                this.objectlist[retVal].SlotValue=slotvalue;
                this.objectlist[retVal].PermanentOwnUID=permVal; 
                this.objectlist[retVal].ObjectRef=null;               
            } else {
                // Luodaan objectindexer kohteet permanentUID tyyppisinä kohteina
                this.objectlist.Add(permVal,new ObjectIndexerField(objtype,objarrayind));
                this.objectlist[permVal].SlotValue=slotvalue;
                this.objectlist[permVal].PermanentOwnUID=permVal;
                this.objectlist[permVal].ObjectRef=null;
            }

            return retVal;
        }
        */

        /// <summary>
        /// Raportointityypit, joiden perusteella objectindexer voi raportoida virheita samalla, kun objekteille luodaan uid tunnuksia
        /// </summary> 
        public enum objectIndexerErrorReportTypes {
            /// <summary>
            ///  Ei virhe raportointia uusien UID tietojen luonnin yhteydessä
            /// </summary>
            NO_REPORTING_0=0,
            /// <summary>
            /// Normaali virhe raportointi, jossa lähetetään virheilmoitus käyttöliittymälle tilanteissa, joissa samalla tunnuksella oli jo kohde olemassa (ei pitäisi olla mahdollista, mutta tilanteita voi tulla eteen esim. ladattujen kohteiden UID tietojen kanssa, jotka pahimmillaan voivat mennä päällekäin nykyisten objektien UID tietojen kanssa)
            /// </summary> 
            NORMAL_ERROR_REPORTING_1=1
        }

        /// <summary> Tämä funktio lisää object indexeriin yksilöllisen objektin tiedot varuilta talteen ja palauttaa käyttäjälle uuden yksilöllisen numeron talletettavaksi objektille </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"> long, tämän kohteen luoman objektin (äiti objekti) uniqrefnum </param>
        /// <param name="objtype"> int, 0 = slotti, 1 = assetti, 100=ConnectionRectangle, 101=ConnectionRectangles, 102=MotherConnectionRectangle, 110=Connection </param>
        /// <param name="objarrayind"> int, arraylist index tai assetin tapauksessa (0=sellersiloobj, 1=holdingsiloobj) </param>
        /// <param name="reporterrorindexing"> int, Raportointityypit, joiden perusteella objectindexer voi raportoida virheita samalla, kun objekteille luodaan uid tunnuksia </param>
        /// <param name="granparentuid"> long, tämän kohteen isoäiti objektin uniqrefnum </param>
        /// <param name="slotvalue"> decimal, slotin arvo, jonka alueella myynti tapahtui </param>
        /// <returns> {long}, palauttaa objektin uniikin yksilöllisen numeron, jolla objektia voidaan etsiä. Jos pienempi kuin 0, niin kyseessä virhekoodi ja koodi kertoo, minkälainen virhe oli kyseessä </returns>
        public long AddObjectToIndexer(string kutsuja, long parentuid, int objtype, int objarrayind, int reporterrorindexing=(int)objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1, long granparentuid=-1, decimal slotvalue=-1)
        {
            long retVal;
            long permVal;
            string functionname="->(OI)AddObjectToIndexer#2";

            retVal=this.GetNewUniqueIdNumber;
            permVal=PermanentUIDGenerator.ReturnPermanentUID(kutsuja+functionname,retVal);
            // Luodaan objectindexer kohteet vanhalla tavalla
            if (this.typeofobjindexer==(int)TypeOfObjectIndexer.RUNNING_INSTANCE_RESETING_INDEXER_0) {
                this.objectlist.Add(retVal,new ObjectIndexerField(objtype,objarrayind));
                this.objectlist[retVal].OwnUID=retVal;
                this.objectlist[retVal].ParentUID=parentuid;
                this.objectlist[retVal].PermanentOwnUID=permVal;
                this.objectlist[retVal].GranParentUID=granparentuid;
                this.objectlist[retVal].ObjectRef=null;
                this.objectlist[retVal].SlotValue=slotvalue;
                this.objectlist[retVal].ShortUID=retVal; // Tämän yksilöllisyyttä ei aina voida taata
                if (this.objectlist.IndexOfKey(parentuid)>-1) {
                    this.objectlist[retVal].PermanentParentUID=this.objectlist[parentuid].PermanentOwnUID;
                } else {
                    if (reporterrorindexing==(int)objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1) {
                        this.proghmi.sendError(kutsuja+functionname,"There was already object in list with created UID! UID:"+retVal+" PermanentUID:"+permVal+" Response: -2",-1110,4,4);
                    }
                    retVal=-2;
                }
                if (granparentuid>-1) {
                    if (this.objectlist.IndexOfKey(granparentuid)>-1) {
                        this.objectlist[retVal].PermanentGranParentUID=this.objectlist[granparentuid].PermanentOwnUID;
                    } else {
                        if (reporterrorindexing==(int)objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1) {
                            this.proghmi.sendError(kutsuja+functionname,"There was already object in list with created UID! UID:"+retVal+" PermanentUID:"+permVal+" Response: -4",-1229,4,4);
                        }
                        retVal=-4;
                    }                    
                }
            } else {
                // Luodaan objectindexer kohteet permanentUID tyyppisinä kohteina
                this.objectlist.Add(permVal,new ObjectIndexerField(objtype,objarrayind));
                this.objectlist[permVal].OwnUID=permVal;
                this.objectlist[permVal].ParentUID=parentuid;
                this.objectlist[permVal].PermanentOwnUID=permVal;
                this.objectlist[permVal].GranParentUID=granparentuid;
                this.objectlist[permVal].ObjectRef=null;
                this.objectlist[permVal].SlotValue=slotvalue;
                this.objectlist[permVal].ShortUID=retVal; // Tämän yksilöllisyyttä ei aina voida taata
                if (this.objectlist.IndexOfKey(parentuid)>-1) {
                    this.objectlist[permVal].PermanentParentUID=this.objectlist[parentuid].PermanentOwnUID;
                } else {
                    if (reporterrorindexing==(int)objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1) {
                        this.proghmi.sendError(kutsuja+functionname,"There was already object in list with created UID! UID:"+permVal+" PermanentUID:"+permVal+" Response: -3",-1111,4,4);
                    }                    
                    retVal=-3;
                }
                if (granparentuid>-1) {
                    if (this.objectlist.IndexOfKey(granparentuid)>-1) {
                        this.objectlist[retVal].PermanentGranParentUID=this.objectlist[granparentuid].PermanentOwnUID;
                    } else {
                        if (reporterrorindexing==(int)objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1) {
                            this.proghmi.sendError(kutsuja+functionname,"There was already object in list with created UID! UID:"+retVal+" PermanentUID:"+permVal+" Response: -5",-1230,4,4);
                        }
                        retVal=-5;
                    }                    
                }                                
            }

            return retVal;
        }

        /// <summary>
        /// Tämä enumeraatio määrittelee tavat joilla objekti voidaan asettaa ObjectIndexeriin. Suurin huomio on sillä, voidaanko referenssiksi asettaa ulkoa päin null vai ei.
        /// </summary> 
        public enum rewriteOldObjectReference {
            /// <summary>
            /// Kirjoittaa objektin referenssin aina siitäkin huolimatta, että annettu referenssi olisi null
            /// </summary>
            ALWAYS_REWRITE_OBJECT_REFERENCE_0=0,
            /// <summary>
            /// Ei kirjoita objektin referenssiä entisen päälle siinä tapauksessa, jos asetettava objektin referenssi on null
            /// </summary>
            NOT_REWRITE_OLD_IF_OBJECT_REF_IS_NULL_1=1,
            /// <summary>
            /// Ei kirjoita objektin referenssiä entisen päälle siinä tapauksessa, jos asetettava objektin referenssi on null, mutta ei myöskään raportoi error tulosta, jos objektin referenssi oli null lähtökohtaisesti
            /// </summary> 
            NOT_REWRITE_OLD_IF_OBJECT_REF_IS_NULL_BUT_NOT_GIVE_ERROR_IF_ORIGINAL_ALSO_NULL_2=2
        }

        /// <summary>
        /// Tämä metodi asettaa objektin referenssin indeksoitavalle kohteelle. Jotta tämän metodin käytöstä on hyötyä, tulee itse AddObjectToIndexer käskyssä olla annettuna tämän objektin tyyppi, jotta tiedetään minkälainen objekti tähän referenssiin on tallennettu
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek"> long, objektin UID, jolla asetettavan kohteen indeksiä etsitään objectlist listalta</param>
        /// <param name="settingobj">object, asetettavan luokan instanssin referenssi, joka halutaan asettaa tälle instanssille talteen</param>
        /// <param name="rewriteoldreference">int, tämä enumeraatio määrittelee tavat joilla objekti voidaan asettaa ObjectIndexeriin. Suurin huomio on sillä, voidaanko referenssiksi asettaa ulkoa päin null vai ei. </param>
        /// <returns>{int}, palauttaa: 3=asetus epäonnistui asetettavan objektin ollessa null näillä asetuksilla, mutta kohde oli myös alunperin null, 2=asetus onnistui, mutta asetettu arvo oli null, 1=asetus onnistui, -1=epämääräinen virhe, -2=ei kohdetta listalla UIDtoseek indeksillä, -3=asetettava objekti on null, eikä moista näillä asetuksilla sallita</returns>
        private int SetObjectToIndexer(string kutsuja, long UIDtoseek, object settingobj, int rewriteoldreference=(int)rewriteOldObjectReference.NOT_REWRITE_OLD_IF_OBJECT_REF_IS_NULL_1)
        {
            int retVal=-1;
            if (this.objectlist.IndexOfKey(UIDtoseek)>-1) {
                if (settingobj!=null) {
                    this.objectlist[UIDtoseek].ObjectRef=settingobj;
                    retVal=1;
                } else {
                    if (rewriteoldreference>=(int)rewriteOldObjectReference.NOT_REWRITE_OLD_IF_OBJECT_REF_IS_NULL_1) {
                        if (rewriteoldreference==(int)rewriteOldObjectReference.NOT_REWRITE_OLD_IF_OBJECT_REF_IS_NULL_1) {
                            retVal=-3;
                        } else {
                            retVal=3;
                        }
                    } else {
                        this.objectlist[UIDtoseek].ObjectRef=settingobj;
                        retVal=2;
                    }
                }
            } else {
                retVal=-2;
            }
            return retVal;
        }
        /// <summary>
        /// Tämä metodi asettaa objektin referenssin indeksoitavalle kohteelle. Jotta tämän metodin käytöstä on hyötyä, tulee itse AddObjectToIndexer käskyssä olla annettuna tämän objektin tyyppi, jotta tiedetään minkälainen objekti tähän referenssiin on tallennettu
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek"> long, objektin UID, jolla asetettavan kohteen indeksiä etsitään objectlist listalta</param>
        /// <param name="settingobj">object, asetettavan luokan instanssin referenssi, joka halutaan asettaa tälle instanssille talteen</param>
        /// <param name="rewriteoldreference">int, tämä enumeraatio määrittelee tavat joilla objekti voidaan asettaa ObjectIndexeriin. Suurin huomio on sillä, voidaanko referenssiksi asettaa ulkoa päin null vai ei. </param>
        /// <returns>{int}, palauttaa: 3=asetus epäonnistui asetettavan objektin ollessa null näillä asetuksilla, mutta kohde oli myös alunperin null, 2=asetus onnistui, mutta asetettu arvo oli null, 1=asetus onnistui, -1=epämääräinen virhe, -2=ei kohdetta listalla UIDtoseek indeksillä, -3=asetettava objekti on null, eikä moista näillä asetuksilla sallita</returns>
        public int SetObjectToIndexerWithErrorReport(string kutsuja, long UIDtoseek, object settingobj, int rewriteoldreference=(int)rewriteOldObjectReference.NOT_REWRITE_OLD_IF_OBJECT_REF_IS_NULL_1)
        {
            int retVal=-10;
            string functionname="->(OI)SetObjectIndexerWithErrorReport";
            if (this.objectlist.IndexOfKey(UIDtoseek)>-1) {
                retVal=this.SetObjectToIndexer(kutsuja+functionname,UIDtoseek,settingobj,rewriteoldreference);
                if (retVal<0) {
                    this.proghmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! Unsuccesful object set! UID:"+UIDtoseek+" Response:"+retVal,-1139,4,4); 
                }
            } else {
                retVal=-2;
                this.proghmi.sendError(kutsuja+functionname,"Fatal error! Problem to set object to objectindexer! No entry found! UID:"+UIDtoseek,-1140,4,4);
            }
            return retVal;
        }

        /// <summary>
        /// Attempts to get an object of type T from the indexer.
        /// </summary>
        /// <typeparam name="T">indexerObjectTypes enumeraatio listasta löytyvän tyyppinen objekti, joka castataan itsensä tyyppisenä takaisin (se tyyppi, mitä käyttäjä on ehdottanut) </typeparam>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota</param>
        /// <param name="UID">long, objektin UID, jolla haettavan kohteen indeksiä etsitään objectlist listalta</param>
        /// <returns>{T} palauttaa käyttäjän ehdottaman objektityypin takaisin tai null jos jotain meni pahasti vikaan. Palauttaa null ja virheen voi lukea GetLastError:in kautta, joka palauttaa ObjectRetrievalError objektin instanssin. </returns>
        public T GetTypedObject<T>(string kutsuja, long UID) where T : class
        {
            string functionname = "->(OI)GetTypedObject";
            string errorMessage="";
            if (objectlist.ContainsKey(UID))
            {
                var entry = objectlist[UID];
                if (entry.ObjectRef is T) {
                    lastError=null;
                    return entry.ObjectRef as T;
                } else {
                    errorMessage=this.proghmi.sendErrorRetString(kutsuja + functionname, "Object type mismatch or incorrect casting. Expected type: " + typeof(T).Name, -1184, 4, 4);
                    lastError = new ObjectRetrievalError(-1184, errorMessage);
                    return null;
                }
            } else {
                errorMessage=this.proghmi.sendErrorRetString(kutsuja + functionname, "No object found with the specified UID: " + UID, -1185, 4, 4);
                lastError = new ObjectRetrievalError(-1185, errorMessage);
                return null;
            }
        }       

        /// <summary>
        /// Tämä metodi hakee objektin referenssin indeksoitavalta kohteelta. Jos kohdetta ei löydy, annetaan virheilmoitus ja palautetaan null.
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota</param>
        /// <param name="UIDtoseek">long, objektin UID, jolla haettavan kohteen indeksiä etsitään objectlist listalta</param>
        /// <returns>object, palauttaa haetun luokan instanssin referenssin tai null, jos kohdetta ei löydy listalta</returns>
        public object GetObjectFromIndexer(string kutsuja, long UIDtoseek)
        {
            string functionname="->(OI)GetObjectFromIndexer";
            if (this.objectlist.ContainsKey(UIDtoseek))
            {
                return this.objectlist[UIDtoseek].ObjectRef;
            }
            else
            {
                // Tässä kohtaa voitaisiin logata virhe tai antaa käyttäjälle virheilmoitus
                this.proghmi.sendError(kutsuja+functionname,"Kohdetta UIDtoseek="+UIDtoseek+" ei löydy indeksistä.",-1102,4,4);
                return null;
            }
        }

        /// <summary> Tämä funktio poistaa object indexeristä sen objektin tiedot, jonka indeksi on delindex </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="delindexkey"> long, indeksin arvo (key), jolla halutaan poistaa objekti listasta </param>
        /// <returns> {int}, palauttaa 1, jos onnistui. Jos pienempi kuin nolla tai yhtäsuuri kuin 0, niin kyseessä virhekoodi - jos 0=kyseisellä indeksillä ei ollut kohdetta listassa, jos -1=tuntematon virhe, jos -2=ei yhtään kohdetta listassa </returns>
        public int DeleteObjectFromIndexer(string kutsuja, long delindexkey)
        {
            int retVal=-1;
            int amo=this.objectlist.Count;
            int inde=-1;

            if (amo>0) {
                inde=this.objectlist.IndexOfKey(delindexkey);
                if (inde>=0) {
                    this.objectlist[inde].ObjectRef=null;
                    this.objectlist.RemoveAt(inde);
                    this.objectlist.TrimExcess();
                    retVal=1;
                } else {
                    retVal=0;
                }
            } else { 
                retVal=-2;
            }

            return retVal;
        }

        /// <summary> Tämä funktio palauttaa objektin tyypin, joka löytyy listasta uniikilla referenssinumerolla uniqrefnum </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="uniqrefnum"> long, uniqrefnum, jolla kohdetta etsitään objectlist listasta </param>
        /// <returns> {int} palauttaa objektin tyypin, jos onnistui. Jos &lt; 0, niin kyseessä virhekoodi - jos -3=kyseisellä indeksillä ei ollut kohdetta listassa, jos -1=tuntematon virhe, jos -2=ei yhtään kohdetta listassa, </returns>        
        public int ReturnObjectType(string kutsuja, long uniqrefnum)
        {
            int amo=-1;
            int retVal=-1;
            int iok=-2;
            
            amo=this.objectlist.Count();

            if (amo>0) {
                iok=this.objectlist.IndexOfKey(uniqrefnum);
                if (iok>=0) {
                    retVal=this.objectlist.ElementAt(iok).Value.ObjectType;
                } else {
                    retVal=-3;
                }
            } else {
                retVal=-2;
            }

            return retVal;
        }

        /// <summary> Tämä funktio palauttaa array indeksin, joka löytyy listasta uniikilla referenssinumerolla uniqrefnum </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="uniqrefnum"> long, uniqrefnum, jolla kohdetta etsitään objectlist listasta </param>
        /// <returns> {int} palauttaa indeksin numeron ObjectArrayIndex muuttujasta, jonka key oli uniqrefnum. -1=epämääräinen virhe, -2=ei yhtään kohdetta listalla ja -3=ei löytynyt kyseisellä uniqrefnumilla kohdetta indeksoituna listalta </returns>
        public int ReturnArrayIndex(string kutsuja, long uniqrefnum)
        {            
            int amo=-1;
            int retVal=-1;
            int iok=-2;
            
            amo=this.objectlist.Count();

            if (amo>0) {
                iok=this.objectlist.IndexOfKey(uniqrefnum);
                if (iok>=0) {
                    retVal=this.objectlist.ElementAt(iok).Value.ObjectArrayIndex;
                } else {
                    retVal=-3;
                }
            } else {
                retVal=-2;
            }

            return retVal;
        }        

    }

    /// <summary> Tämä luokka tallentaa yksittäisten kohteiden tietoja objectlistin tietoalkoina, joista on hyötyä kohteiden etsinnässä ja niiden päivittämisessä </summary>
    public class ObjectIndexerField
    {
        /// <summary>
        /// Kohteen slotin arvo, mikäli sellainen on kohteelle olemassa
        /// </summary>
        public decimal SlotValue { get; set; }

        /// <summary> 0=slotti, 1=assetti, 100=ConnectionRectangle, 101=ConnectionRectangles, 102=MotherConnectionRectangle, 110=Connection. Katso täysi lista indexerObjectTypes enumeraatiosta </summary>
        public int ObjectType { get; set; }

        /// <summary>
        ///  Tämä property sisältää itse kyseisen luodun objektin referenssin, jonka kautta itse objektia voidaan käyttää objectindexerin kautta suoraan, mikäli tämä ObjectRef on asetettu kohteelle. Tämä property kulkee käsikädessä ObjectType propertyn kanssa, jonka indexerObjectTypes enumeraatiosta löytyvät luokat, jonka tyyppinen tämä objekti voi olla
        /// </summary>
        private object objectref;

        /// <summary>
        ///  Tämä property sisältää itse kyseisen luodun objektin referenssin, jonka kautta itse objektia voidaan käyttää objectindexerin kautta suoraan, mikäli tämä ObjectRef on asetettu kohteelle. Tämä property kulkee käsikädessä ObjectType propertyn kanssa, jonka indexerObjectTypes enumeraatiosta löytyvät luokat, jonka tyyppinen tämä objekti voi olla
        /// </summary>
        public object ObjectRef {
            get { return objectref; }
            set { objectref = value; }
        }

        /// <summary> Mihin arrayseen kuuluu - esim. onko invslotteja vai normislotteja tahi sellerobjsilo vai holdingsilo jne. </summary>
        public int ObjectArrayIndex { get; set; } 

        /// <summary> Voidaan tallettaa tarvittaessa referenssi assetsellerobjektista </summary>
        public AssetSellerObject AssetSellerObj { get; set; } 

        /// <summary> Voidaan tallettaa tarvittaessa referenssi assetholdingobjectista </summary>
        public AssetSiloObject AssetSiloObj { get; set; } 

        /// <summary> Voidaan tallentaa tarvittaessa referenssi slotista (OneSlot) </summary>
        public OneSlot SlotObj { get; set; }

        /// <summary> Tämän kohteen oman uniqrefnum </summary>
        public long OwnUID { get; set; }

        /// <summary> Tämän kohteen vanhemman uniqrefnum </summary>
        public long ParentUID { get; set; }

        /// <summary> Tämän kohteen isovanhemman uniqrefnum </summary>
        public long GranParentUID { get; set; }

        /// <summary>
        /// Tämän kohteen oma PermanentUID
        /// </summary>
        public long PermanentOwnUID { get; set; }

        /// <summary>
        /// Vanhemman PermanentUID
        /// </summary>
        public long PermanentParentUID { get; set; }

        /// <summary>
        /// Isovanhemman PermanentUID
        /// </summary>
        public long PermanentGranParentUID { get; set; }

        /// <summary>
        /// OwnUID silloin lyhyemmässä muodossa esim. viitteiden talletusjärjestelmää varten, kun meillä on PermanentUID objectlist:in avaimena ja haluamme tallettaa lyhyen UID tiedon jonnekin (käytännössä retVal AddObjectIndexer käskyissä)
        /// </summary>
        public long ShortUID { get; set; }

        public ObjectIndexerField(int objtype, int objarraytyp)
        {
            this.ObjectType=objtype;
            this.ObjectArrayIndex=objarraytyp;
            this.GranParentUID=-1;
            this.PermanentGranParentUID=-1;
        }
    }

    /// <summary>
    /// Tämä luokka luo uuden permanent tyyppisen uid luvun annettujen tietojen perusteella
    /// </summary>
    public class PermanentUIDGenerator
    {
        /// <summary>
        /// Tämä muuttuja kertoo miten suurella luvulla kerrotaan saatu minuuttimäärä ja kuinka monta uniikkia id numeroa mahtuu jokaiselle minuutille
        /// </summary>
        public static long MinuteMultiplier=100000000000;

        /// <summary>
        /// Tämä metodi luo permanentUID tyyppisen UID luvun, joka on yksilöllinen kohteella ja kyseisellä yksilöllisellä kohteella pystytään tunnistamaan kohde toisesta esim. ladatessa vanhoja tiedostoja.
        /// Luku saadaan kun minuutit laskien vuoden 2020 alusta kerrotaan minutemultiplier luvulla ja siihen lisätään sen minuutin yksilöllinen uid indeksi luku, jolla saadaan koko yksilöllinen permanentUID aikaiseksi. Maksimissaan yhdelle minuutille voidaan luoda MinuteMultiplier määrän verran UID lukuja.
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="index"> long, indeksiluku joka lisätään minuuttilukuun. </param>
        /// <returns> {long} palauttaa permanentUID tyyppisen luvun, jota ei pitäisi olla millään mulla kohteella eikä mikään muu kohde tulisi kaiken järjen mukaan koskaan saadakkaan samaa UID lukua, koska aika kulkee koko ajan eteenpäin. </returns>
        public static long ReturnPermanentUID(string kutsuja, long index)
        {
            DateTime startDate = new DateTime(2020, 1, 1);
            DateTime now = DateTime.Now;

            // Lasketaan kuluneet täydet minuutit vuoden 2020 alusta
            TimeSpan timeDifference = now - startDate;
            long fullMinutes = (long)timeDifference.TotalMinutes;

            // Lasketaan uniikki ID
            long uniqueId = fullMinutes * MinuteMultiplier + index;
            return uniqueId;
        }
    }