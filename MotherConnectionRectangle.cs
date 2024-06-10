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
using System.Security.Policy;
using System.Net.Http.Headers;

    /// <summary> Luokka, joka pitää sisällään ConnectionRectanglen pääkohteille (ylätason laatikko) </summary>
    public class MotherConnectionRectangle
    {
        /// <summary> Laatikon oma uniqrefnum </summary>
        public long OwnUID { get; set; }

        /// <summary>
        /// Äitiobjektin UID, jolla saadaan tietoomme ActionCentreConstructionHandlerin instanssi on kyseessä
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Parent objektin äitiobjektin UID
        /// </summary>
        public long GranParentUID { get; set; }        

        /// <summary>
        /// Tämän objektin latauksen yhteydessä löydetty OwnUID tallennettuna, koska muuten voi syntyä ristiriitoja yksilöllisten tunnusten kanssa.
        /// </summary>
        public long LoadedOwnUID { get; set; }

        /// <summary> Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </summary>
        private ObjectIndexer objectindexerref;

        /// <summary> Tämän luokkan instanssi pitää itse päälaatikon tietoja tallessa </summary>
        public ConnectionRectangle mainboxConnectionRect; 

        /// <summary> Tämän luokan instanssi pitää tallessa kaikki ne pienet laatikot, jotka on luotu päälaatikon alle </summary>
        public ConnectionRectangles smallBoxRectangles;

        /// <summary> Käyttöliittymäluokan referenssi </summary>
        private ProgramHMI programhmi;

        /// <summary>
        /// Tämä luokan instanssi hoitaa useiden luokkien kohdalla parametrien printtauksen yhdellä vakioidulla tavalla
        /// </summary>
        private ParamPrinter paramprintteri;

        /// <summary> Referenssi Connectionshandler luokkaan, joka pitää connection objektit järjestyksessä listassa </summary>
        private ConnectionsHandler connectionshandler;

        /// <summary> Tämä objekti pitää sisällään kaikki erilliset UI komponentit, jotka on liitetty varsinaiseen MotherRectangleen sekä niihin liitetyt parametrien arvot erillisessä aliluokassaan </summary>
        public StoredUIObjectsForActionCentre StoredUIcomps {
            get;
            set;
        }

        /// <summary> SortedList long, ConnectionRectangle , referenssi lista kaikista laatikoista jotka on rekisteröity </summary>  
        private SortedList<long, ConnectionRectangle> listofallboxesrefs;
        
        /// <summary>
        ///  Referenssi listaan kaikista Motherconnectionrectangleista
        /// </summary>
        private SortedList<long, MotherConnectionRectangle> listofallmotherrefs;

        /// <summary>
        /// Luodun blokin tyyppi - eli sama kuin boxtype ja sama kuin blockTypeEnum ActionCentre luokassa
        /// </summary>
        private int blocktype;
        /// <summary> Luodun blokin tyyppi - eli sama kuin boxtype ja sama kuin blockTypeEnum ActionCentre luokassa </summary>
        public int BlockType {
            get { return this.blocktype; }
            set { this.blocktype = value; }
        }

        /// <summary>
        /// Parametrisista toimintapoluista vastaavan luokan referenssi, jolla voi hyödyntää blokkirakenteen toimintaobjekteja. Lisäksi säilyttää toimintablokkien sisältöjä.
        /// </summary>
        private ActionCentre actioncent;

        /// <summary>
        /// Parametrisista toimintapoluista vastaavan luokan referenssi, jolla voi hyödyntää blokkirakenteen toimintaobjekteja. Lisäksi säilyttää toimintablokkien sisältöjä.
        /// </summary>
        public ActionCentre ReturnActionCentreRef {
            get { return this.actioncent; }
        }

        /// <summary> Onko tämä luokka initialisoitu oikein, että voimme jatkossa käyttää sitä haluamallamme tavalla? Jos luku on pienempi kuin 1, niin luokkaa ei vielä ole initialisoitu oikealla tavalla </summary>
        public int isinitialized=-1;

        /// <summary> Palauttaa initialisointiluvun. Jos luku on 1=luokan initialisointi on kunnossa. Muut pienemmät luvut tarkoittavat, että initialisointi ei vielä on mennyt loppuun saakka </summary>
        public int IsInitialized {
            get { return this.isinitialized; }
        }

        /// <summary> Palauttaa true, jos tämän luokan initialisointi on kunnossa, muussa tapauksessa false </summary>
        public bool InitializationOkay {
            get {
                if (this.isinitialized>0) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        private int iscomponentsregistered=0;
        /// <summary>
        /// Tarkistaa, onko jo UI komponentit rekisteröity - tarkoittaa onko komponentit jo luotu ja asetettu
        /// </summary>
        public int IsUICompsAlreadyRegistered {
            get { return this.iscomponentsregistered; }
        }

        private int iscomponentregisteredtreshold=1;
        /// <summary> Palauttaa true, jos tämän luokan UI rekisteröinti on hoidettu, muussa tapauksessa false </summary>
        public bool IsUIRegisterationOkay {
            get {
                if (this.iscomponentsregistered>=this.iscomponentregisteredtreshold) {
                    return true;
                } else {
                    return false;
                }
            }
        }
        /// <summary>
        /// Tämä referenssi pitää sisällään tarvittavat tiedot ParseJSON objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja
        /// </summary>
        private JsonParsingStruct parsestructure;

        /// <summary> int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </summary>
        private int createuicomps=0;

        /// <summary>
        /// ActionCentre tyyppisen blokin UID. Kyseinen blokki on geneerinen toimintablokki, jonka alle sijoittuu varsinainen spesifi blokin toimintatyyppi. UID osoittaa BlockContainer nimiseen luokkaan, joka pitää sisällään kohteen UID:n objektin tyypin sekä itse objektin.
        /// </summary>
        private long actioncentreblockUID=-1;

        /// <summary>
        /// ActionCentre tyyppisen blokin UID. Kyseinen blokki on geneerinen toimintablokki, jonka alle sijoittuu varsinainen spesifi blokin toimintatyyppi. UID osoittaa BlockContainer nimiseen luokkaan, joka pitää sisällään kohteen UID:n objektin tyypin sekä itse objektin.
        /// </summary>
        public long ActionCentreBlockUID {
            get { return this.actioncentreblockUID; }
        }

        /// <summary> Constructor luokalle, joka vastaa äitilaatikoiden ominaisuuksien tallentamisesta </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="ownuid"> long, tämän kyseisen laatikon uniqrefnum </param>
        /// <param name="parentuid"> long, Äitiobjektin UID, jolla saadaan tietoomme ActionCentreConstructionHandlerin instanssi on kyseessä </param>
        /// <param name="granparentuid"> long, Parent objektin äitiobjektin UID </param> 
        /// <param name="jsonpstruct"> JsonParsingStruct, tämä referenssi pitää sisällään tarvittavat tiedot ParseJSON objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja </param>
        /// <param name="objindref"> ObjectIndexer, Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </param>
        /// <param name="prograhmi"> ProgramHMI, referenssi käyttöliittymäluokkaan </param>
        /// <param name="actcen"> ActionCentre, Parametrisista toimintapoluista vastaavan luokan referenssi, jolla voi hyödyntää blokkirakenteen toimintaobjekteja. Lisäksi säilyttää toimintablokkien sisältöjä. </param>
        /// <param name="pprinte"> ParamPrinter, referenssi parametri printteri luokan instanssiin </param>
        /// <param name="connhan"> ConnectionsHandler, referenssi luokkaan, joka pitää connection objektit järjestyksessä listassa </param>
        /// <param name="listofallboxes"> SortedList long, ConnectionRectangle , lista kaikista laatikoista jotka on rekisteröity</param>
        /// <param name="listofallmotherr"> SortedList long,MotherConnectionRectangle, lista kaikista pää/äiti laatikoista </param>
        /// <param name="boxtype"> int, Luodun blokin tyyppi - eli sama kuin boxtype ja sama kuin blockTypeEnum ActionCentre luokassa </param>
        /// <param name="creategraphcomp"> int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </param>
        /// <returns> {void} </returns>        
        public MotherConnectionRectangle(string kutsuja, long ownuid, long parentuid, long granparentuid, JsonParsingStruct jsonpstruct, ObjectIndexer objindref, ProgramHMI prograhmi, ActionCentre actcen, ParamPrinter pprinte, ConnectionsHandler connhan, SortedList<long, ConnectionRectangle> listofallboxes, SortedList<long, MotherConnectionRectangle> listofallmotherr, int boxtype, int creategraphcomp=1)
        {
            string functionname="->(MCR)Constructor";
            this.OwnUID=ownuid;
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.programhmi=prograhmi;
            this.parsestructure=jsonpstruct;
            this.objectindexerref=objindref;
            this.connectionshandler=connhan;
            this.actioncent=actcen;
            this.listofallboxesrefs=listofallboxes;
            this.listofallmotherrefs=listofallmotherr;
            this.createuicomps=creategraphcomp;

            if (this.objectindexerref.objectlist.IndexOfKey(this.OwnUID)>-1) {
                int resp=this.objectindexerref.SetObjectToIndexerWithErrorReport(kutsuja+functionname,this.OwnUID,this);
                if (resp<0) {
                    this.programhmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! Unsuccesful object set! UID:"+ownuid+" Response:"+resp,-1122,4,4); 
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error! Problem to set object to objectindexer! No entry found! UID:"+this.OwnUID,-1118,4,4);
            }

            //this.paramprintteri=pprinte;
            this.paramprintteri=new ParamPrinter(kutsuja+functionname,this.programhmi,pprinte.merkkijonokasittelija); // Tarvitsee mahdollisesti oman instanssinsa paramprintteristä, jotta saa printattua kaikki kohteet parametrilistasta

            long mainboxconnectionrectuid=-1;
            this.mainboxConnectionRect=new ConnectionRectangle(kutsuja+functionname,this.OwnUID,this.ParentUID,this.objectindexerref,this.programhmi,this.paramprintteri, this.connectionshandler,"",out mainboxconnectionrectuid,creategraphcomp); // Pitää päälaatikon tietoja sisällään
            this.smallBoxRectangles=new ConnectionRectangles(kutsuja+functionname,this.OwnUID,this.ParentUID, this.parsestructure,this.objectindexerref,this.programhmi,this.paramprintteri, this.connectionshandler,creategraphcomp); // Hallitsee päälaatikon sisälle printattavien pienempien laatikoiden tietoja 
            this.StoredUIcomps = new StoredUIObjectsForActionCentre(kutsuja+functionname,this.OwnUID,this.programhmi,pprinte,this.objectindexerref,creategraphcomp); // Pitää päälaatikkoon printattavien graafisten komponenttien tietoja sisällään
            this.blocktype=boxtype;
            // Luodaan tätä blokkia vastaava blocktype blokki ActionCentre luokassa ja tallennetaan itsellemme kyseisen toimintoblokin UID
            this.actioncentreblockUID=this.actioncent.CreateBlock(kutsuja+functionname,this.blocktype,this);
            if (this.actioncentreblockUID>=0) {

                this.isinitialized++; // Siirtyy initialisoinnissa kohtaan 0
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error! Error to create Block! Response:"+this.actioncentreblockUID,-1302,4,4);
            }
        }

        /// <summary>
        /// Kopioi tulopuolen kahvojen arvot kutsumalla iterationclass 1 ja 2 arvoja vastaavia metodeja.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <returns>{int} Palauttaa 1, jos kaikki toimenpiteet onnistuivat. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos jokin toimenpide epäonnistui.</returns>
        public int CopyAllInputHandlesBlockAtomValues(string kutsuja)
        {
            string functionname = "->(MCR)CopyAllInputHandlesBlockAtomValues";
            for (int iterationclass = (int)ConnectionRectangles.connectionBoxType.MIN_VALUE_INDEX; iterationclass <= (int)ConnectionRectangles.connectionBoxType.MAX_VALUE_INDEX_FOR_INCOMING_HANDLES; iterationclass++) {
                int result = this.smallBoxRectangles.CopyAllBlockAtomValuesByIterationClass(kutsuja + functionname, iterationclass);
                if (result < 0) {
                    this.programhmi.sendError(kutsuja + functionname, "Failed to copy BlockAtomValues for iteration class: " + iterationclass + " Response:" + result, -1329, 4, 4);
                    return result; // Palautetaan ensimmäinen negatiivinen virhekoodi
                }
            }
            return 1; // Kaikki toimenpiteet onnistuivat
        }

        /// <summary>
        /// Tämä metodi palauttaa tämän koko MotherConnectionRectangle blokin ja sen alablokkien tiedot JSON tyyppisenä objektina tallennusta varten
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns>{string}, Palauttaa tätä blokkia koskevat tiedot JSON objektina</returns>
        public string ReturnThisBlockSavingParametersAsJSON(string kutsuja)
        {
            string functionname="->(MCR)ReturnThisBlockSavingParametersAsJSON";
            string retVal="";

            if (this.InitializationOkay==true) {
                retVal=this.ReturnThisBlockJSONParameters(kutsuja+functionname,ParamNameLists.motherconnSavingParamNames);
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Not initialized in proper way! Initialization info:"+this.IsInitialized,-867,4,4);
            }
            return retVal;
        }

        /// <summary>
        /// Tämä metodi palauttaa tämän koko MotherConnectionRectangle blokin kaikki tiedot ja sen alablokkien tiedot JSON tyyppisenä objektina printtausta varten
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns>{string}, Palauttaa tätä blokkia koskevat tiedot JSON objektina</returns>
        public string ReturnThisBlockAllParametersAsJSON(string kutsuja)
        {
            string functionname="->(MCR)ReturnThisBlockAllParametersAsJSON";
            string retVal="";

            if (this.InitializationOkay==true) {
                retVal=this.ReturnThisBlockJSONParameters(kutsuja+functionname,ParamNameLists.motherconnParamNames);
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Not initialized in proper way! Initialization info:"+this.IsInitialized,-890,4,4);
            }
            return retVal;
        }        

        /// <summary>
        /// Tämä metodi palauttaa tämän koko MotherConnectionRectangle blokin ja sen alablokkien tiedot JSON tyyppisenä objektina
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="jsonparameters">List string tyyppisen listan referenssi, jossa on kaikki parametrinimet jotka halutaan printattavan JSON objektiin</param>
        /// <returns>{string}, Palauttaa tätä blokkia koskevat tiedot JSON objektina</returns>
        public string ReturnThisBlockJSONParameters(string kutsuja, List<string> jsonparameters)
        {
            string functionname="->(MCR)ReturnThisBlockJSONParameters";
            string retVal="";

            if (this.InitializationOkay==true) {
                if (jsonparameters!=null) {
                    if (jsonparameters.Count>0) {
                        this.paramprintteri.SetMotherConnectionRectangleObjectToPrint(this);
                        retVal=this.paramprintteri.MyOwnParamPrint(kutsuja+functionname,jsonparameters,(int)ParamPrinter.myOwnTypePrintingEnum.JSON_OBJECT_WITH_PARAM_NAMES_AND_VALUES_2);
                    } else {
                        this.programhmi.sendError(kutsuja+functionname,"Parameter list didn't contain any parameter name (JSONparameters)!",-889,4,4);
                        retVal="ERROR=-110";
                    }
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"Parameter list was null (JSONparameters)!",-888,4,4);
                    retVal="ERROR=-111";
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Not initialized in proper way! Initialization info:"+this.IsInitialized,-887,4,4);
            }
            return retVal;
        }        

        /// <summary> Tämä metodi rekisteröi ne käyttöliittymäkomponentit, jotka printataan MotherRectanglen faceplateen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="blockn"> TextBox, komponentti, josta löytyy blokin nimi </param>
        /// <param name="route"> TextBox, komponentti josta löytyy RouteId </param>
        /// <param name="grouph"> Combobox, komponentti johon on talletettu blokissa ryhmän valinta </param>        
        /// <param name="valueh"> Combobox, komponentti johon on talletettu blokissa tehty valinta </param>
        /// <param name="printval1"> TextBlock, komponentti johon voi printata jotain rivi 1 </param> 
        /// <param name="printval2"> TextBlock, komponentti johon voi printata jotain rivi 2 </param>       
        /// <returns> {void} </returns>  
        public void RegisterUIComponents(string kutsuja, TextBox blockn, TextBox route, ComboBox grouph, ComboBox valueh, TextBlock printval1, TextBlock printval2)
        {
            string functionname="->(MCR)RegisterUIComponets";
            if (this.IsUICompsAlreadyRegistered<this.iscomponentregisteredtreshold) {
                this.StoredUIcomps.RegisterUIComponents(kutsuja,blockn,route,grouph,valueh,printval1,printval2);
                this.isinitialized++; // Siirtyy initialisoinnissa kohtaan 1
                this.iscomponentsregistered++;
            } else {
                this.programhmi.sendError(kutsuja+functionname, "UI components already registered! Reregisteration wasn't aloud!",-109,4,4);
            }
        }  

        /// <summary>
        /// Tämä metodi asettaa mainboxConnectionRect objektille tiedot paikalleen JSON objektin perusteella, joka on annettu avattuna SortedDictionaryna motherconnectionjsonobj muuttujalle
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>  
        /// <param name="parsestruct">JsonParsingStruct, referenssi instanssiin, jolla pystytään avaamaan JSON objekti ja saadaan uniikkeja id tunnuksia eri parse tapahtumiin</param>
        /// <param name="motherconnectionjsonobj">SortedDictionary string, string - johon on annettu avattuna koko ladattu JSON motherconnection objekti</param>
        /// <param name="isloaded"> int, jos 0, niin ei olle tietojen latauksesta kyse ja jos 1, niin UID tiedot menevät erillisiin Loaded muuttujiin varsinaisten muuttujien sijasta </param>
        /// <returns> {void} </returns>
        public void LoadSmallRectangleParams(string kutsuja, JsonParsingStruct parsestruct, SortedDictionary<string, string> motherconnectionjsonobj, int isloaded)
        {
            string functionname="->(ACUI)LoadSmallRectangleParams";
            string mandatoryparam="smallBoxRectangles";
            SortedDictionary<string, string> smallboxrects;
            long rememparseuid=-1;

            if (parsestruct!=null) {
                if (motherconnectionjsonobj!=null) {
                    if (motherconnectionjsonobj.ContainsKey(mandatoryparam)==true) {                        
                        rememparseuid=parsestruct.Parserunninguid;
                        smallboxrects=parsestruct.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,parsestruct.Parserunninguid.ToString(),motherconnectionjsonobj[mandatoryparam],-1); // Tässä -1 tarkoittaa, ettei tehdä debug printtausta
                        parsestruct.Parserunninguid++;

                        if (smallboxrects!=null) {
                            this.paramprintteri.SetConnectionRectanglesObjectToPrint(this.smallBoxRectangles);
                            this.paramprintteri.SetConnectionRectanglesParamValues(kutsuja+functionname,smallboxrects,isloaded);
                        } else {
                            this.programhmi.sendError(kutsuja+functionname,"Smallboxrectangledictionary was null!",-954,4,4);
                        }
                        parsestruct.Parsejson.CloseParsing(kutsuja+functionname,rememparseuid.ToString());

                    } else {
                        this.programhmi.sendError(kutsuja+functionname,"MotherconnectionJsonObj didn't contain mandatory parameter! Param:"+mandatoryparam,-955,4,4);
                    }
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"MotherconnectionJsonObj was null!",-956,4,4);
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Parsestruct was null!",-957,4,4);
            }
        } 

        /// <summary>
        /// Tämä metodi asettaa mainboxConnectionRect objektille tiedot paikalleen JSON objektin perusteella, joka on annettu avattuna SortedDictionaryna motherconnectionjsonobj muuttujalle
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>  
        /// <param name="parsestruct">JsonParsingStruct, referenssi instanssiin, jolla pystytään avaamaan JSON objekti ja saadaan uniikkeja id tunnuksia eri parse tapahtumiin</param>
        /// <param name="motherconnectionjsonobj">SortedDictionary string, string - johon on annettu avattuna koko ladattu JSON motherconnection objekti</param>
        /// <param name="isloaded"> int, jos 0, niin ei olle tietojen latauksesta kyse ja jos 1, niin UID tiedot menevät erillisiin Loaded muuttujiin varsinaisten muuttujien sijasta </param>
        /// <returns> {void} </returns>
        public void LoadMainBoxConnectionRectParams(string kutsuja, JsonParsingStruct parsestruct, SortedDictionary<string, string> motherconnectionjsonobj, int isloaded)
        {
            string functionname="->(ACUI)LoadMainBoxConnectionRectParams";
            string mandatoryparam="mainboxConnectionRect";
            SortedDictionary<string, string> mainboxconndict;
            long rememparseuid=-1;

            if (parsestruct!=null) {
                if (motherconnectionjsonobj!=null) {
                    if (motherconnectionjsonobj.ContainsKey(mandatoryparam)==true) {                        
                        rememparseuid=parsestruct.Parserunninguid;
                        mainboxconndict=parsestruct.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,parsestruct.Parserunninguid.ToString(),motherconnectionjsonobj[mandatoryparam],-1); // Tässä -1 tarkoittaa, ettei tehdä debug printtausta
                        parsestruct.Parserunninguid++;

                        if (mainboxconndict!=null) {
                            this.paramprintteri.SetConnectionRectangleObjectToPrint(this.mainboxConnectionRect);
                            this.paramprintteri.SetConnectionRectangleParamValues(kutsuja+functionname,mainboxconndict,isloaded);
                        } else {
                            this.programhmi.sendError(kutsuja+functionname,"MainboxDictionary was null!",-948,4,4);
                        }
                        parsestruct.Parsejson.CloseParsing(kutsuja+functionname,rememparseuid.ToString());

                    } else {
                        this.programhmi.sendError(kutsuja+functionname,"MotherconnectionJsonObj didn't contain mandatory parameter! Param:"+mandatoryparam,-949,4,4);
                    }
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"MotherconnectionJsonObj was null!",-950,4,4);
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Parsestruct was null!",-951,4,4);
            }
        } 

        /// <summary>
        /// Tämä metodi asettaa storedUIcomps objektille tiedot paikalleen JSON objektin perusteella, joka on annettu avattuna SortedDictionaryna motherconnectionjsonobj muuttujalle
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>  
        /// <param name="parsestruct">JsonParsingStruct, referenssi instanssiin, jolla pystytään avaamaan JSON objekti ja saadaan uniikkeja id tunnuksia eri parse tapahtumiin</param>
        /// <param name="motherconnectionjsonobj">SortedDictionary string, string - johon on annettu avattuna koko ladattu JSON motherconnection objekti</param>
        /// <param name="isloaded"> int, jos 0, niin ei olle tietojen latauksesta kyse ja jos 1, niin UID tiedot menevät erillisiin Loaded muuttujiin varsinaisten muuttujien sijasta </param>
        /// <returns> {void} </returns>
        public void LoadStoredUIObjectParams(string kutsuja, JsonParsingStruct parsestruct, SortedDictionary<string, string> motherconnectionjsonobj, int isloaded)
        {
            string functionname="->(ACUI)LoadStoredUIObjectParams";
            string mandatoryparam="storedUIcomps";
            SortedDictionary<string, string> storeduidict;
            long rememparseuid=-1;

            if (parsestruct!=null) {
                if (motherconnectionjsonobj!=null) {
                    if (motherconnectionjsonobj.ContainsKey(mandatoryparam)==true) {                        
                        rememparseuid=parsestruct.Parserunninguid;
                        storeduidict=parsestruct.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,parsestruct.Parserunninguid.ToString(),motherconnectionjsonobj[mandatoryparam],-1); // Tässä -1 tarkoittaa, ettei tehdä debug printtausta
                        parsestruct.Parserunninguid++;

                        if (storeduidict!=null) {
                            this.paramprintteri.SetStoredUIObjectsToPrint(this.StoredUIcomps);
                            this.paramprintteri.SetStoredUIObjectParamValues(kutsuja+functionname,storeduidict,isloaded);
                        } else {
                            this.programhmi.sendError(kutsuja+functionname,"StoredUIDictionary was null!",-940,4,4);
                        }
                        parsestruct.Parsejson.CloseParsing(kutsuja+functionname,rememparseuid.ToString());

                    } else {
                        this.programhmi.sendError(kutsuja+functionname,"MotherconnectionJsonObj didn't contain mandatory parameter! Param:"+mandatoryparam,-941,4,4);
                    }
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"MotherconnectionJsonObj was null!",-942,4,4);
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Parsestruct  was null!",-943,4,4);
            }
        }              

        /// <summary> Tämä metodi päivittää ison laatikon sijainnin sekä sen sisällä olevien kaikkien pikkulaatikoiden sijainnit, kun isoa laatikkoa siirretään toiseen kohtaan vetämällä </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>        
        /// <param name="newTop"> double, ison "äitilaatikon" uusi korkeusasema </param>
        /// <param name="newLeft"> double, ison "äitilaatikon" uusi leveysasema </param>
        /// <param name="mainwidth"> double, päälaatikon leveys </param>
        /// <param name="smallboxwidth"> double, pikkulaatikon leveys </param>
        /// <param name="heightLeft"> double, vasemman laidan pikkulaatikkojen korkeus </param>
        /// <param name="heightRight"> double, oikean laidan pikkulaatikkojen korkeus </param>
        /// <param name="infoOffsetLeft"> double, vasemman laidan offset infopanelille </param>
        /// <param name="infoOffsetTop"> double, ylälaidan offset infopanelille </param>
        /// <param name="letteroffsetleft"> double, laatikon teksitikirjaimen offset yhdyslaatikolle sen vasemmasta reunasta </param>
        /// <param name="letteroffsettop"> double, laatikon teksitikirjaimen offset yhdyslaatikolle sen yläreunasta </param>
        /// <returns> {void} </returns>
        public void UpdatePositionsDuringDrag(string kutsuja, double newTop, double newLeft, double mainwidth, double smallboxwidth, double heightLeft, double heightRight, double infoOffsetLeft, double infoOffsetTop, double letteroffsetleft, double letteroffsettop)
        {
            string functionname="->(MCR)UpdatePositionsDuringDrag";
            if (this.InitializationOkay==true) {
                this.mainboxConnectionRect.UpdatePositionsDuringDrag(kutsuja+functionname,newTop,newLeft, letteroffsetleft, letteroffsettop);
                this.smallBoxRectangles.UpdatePositionsDuringDrag(kutsuja+functionname,newTop,newLeft,mainwidth,smallboxwidth,heightLeft,heightRight, letteroffsetleft, letteroffsettop);
                this.StoredUIcomps.UpdateInfoPanelDuringDrag(kutsuja+functionname,newTop+infoOffsetTop,newLeft+infoOffsetLeft);
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Not initialized in proper way! Initialization info:"+this.IsInitialized,-749,4,4);
            }
        }

        /// <summary>
        /// Tämä metodi poistaa kaikki tähän (pääkomponenttiin) kohteeseen kiinnitetyt muut kohteet
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="origcanvas"> Canvas, se canvas josta kyseinen laatikko poistetaan</param>
        /// <returns> {void} </returns>
        public void DeleteMotherConnectionRectangleComponents(string kutsuja, Canvas origcanvas)
        {
            string functionname="->(MCR)DeleteMotherConnectionRectangleComponents";
            if (this.StoredUIcomps.RegisteredObject==true) {
                this.StoredUIcomps.RemoveStoredUIObjects(kutsuja,origcanvas);
            }
            this.smallBoxRectangles.DeleteConnectionRectangles(kutsuja+functionname,origcanvas,this.listofallboxesrefs); // Poistetaan kaikki yhdistyslaatikot sekä niiden yhdistysobjektit
            this.mainboxConnectionRect.RemoveRectangle(kutsuja+functionname,origcanvas,this.listofallboxesrefs); // Poistetaan itse motherConnectionReactangle
            this.listofallmotherrefs.Remove(this.OwnUID); // Poistetaan kohde listasta, johon on kerätty vain pää/äitikohteiden laatikoiden tiedot 
        }
    }

    /*
    public class MotherConnectionRectangleData
    {
        public long ParentUID { get; set; }
        public long ThisInstanceOwnUID { get; set; }
        public bool IsSelected { get; set; }
        public ConnectionType ConnType { get; set; }
        // Muut olennaiset tietomallin tiedot ja logiikka

        public MotherConnectionRectangleData(string kutsuja, long parentUID, long thisInstanceOwnUID, ConnectionType connType)
        {
            ParentUID = parentUID;
            ThisInstanceOwnUID = thisInstanceOwnUID;
            ConnType = connType;
            IsSelected = false;
        }

        // Lisää tähän muita metodeja ja logiikkaa, jotka liittyvät tietomalliin, mutta eivät käyttöliittymään
    }
    */ 


    /// <summary>
    /// Tämä luokka pitää sisällään StoredUIObjectsForActionCentre luokan parametrien yksittäiset tärkeät tiedot, jotta niitä voidaan käyttää ilman, että meillä on UI käytössä
    /// </summary> 
    public class StoredUIComponentParamValues
    {
        /// <summary> Blokin nimi blockname muuttujassa StoredUIObjectsForActionCentre objektissa, josta löytyy blokin nimi </summary>
        public string BlockName { get; set; }

        /// <summary> RouteId:n luku StoredUIObjectsForActionCentre objektissa, eli käytännössä sama tieto kuin Slotlistin AltRoute </summary>
        public int RouteId { get; set; }

        /// <summary> SelectedGroupCombo Combobox:in valittu arvo valittu ryhmäarvo kullakin hetkellä StoredUIObjectsForActionCentre objektissa </summary>
        public string SelectedGroup { get; set; }

        /// <summary> SelectedGroupCombo Combobox:in valittu operaattoriarvo kullakin hetkellä StoredUIObjectsForActionCentre objektissa </summary>
        public string SelectedOperator { get; set; }

        /// <summary> Printattava arvo 1 printvalue1 muuttujassa StoredUIObjectsForActionCentre objektissa, johon voimme printata jotain näkymään itse blockiin - valuearea 1 </summary>
        public string PrintValue1 { get; set; }

        /// <summary> Printattava arvo 2 printvalue2 muuttujassa StoredUIObjectsForActionCentre objektissa, johon voimme printata jotain näkymään itse blockiin - valuearea 2 </summary>
        public string PrintValue2 { get; set; }

        /// <summary>
        /// Tämän kohteen oma UID
        /// </summary>
        public long ThisInstanceOwnUID { get; set; }

        /// <summary> Äitiobjektin (StoredUIObjectsForActionCentre) uniqrefnum eli UID </summary>
        public long ParentUID { get; set; }

        /// <summary> Laatikon oma (MotherRectanglen) uniqrefnum </summary>
        public long GraphCompOverParentUID { get; set; }

        /// <summary>
        /// Latauksen yhteydessä ollut laatikon oma (MotherRectanglen) uniqrefnum
        /// </summary>
        public long GraphCompLoadedOverParentUID { get; set; }

        /// <summary>
        /// Tämän objektin vanhemman UID, eli käytännössä StoredUIObjectsForActionCentre instanssin oma UID
        /// </summary>
        public long GraphCompParentUID { get; set; }        

        /// <summary>
        /// Constructor luokalle, joka pitää sisällään StoredUIObjectsForActionCentre luokan parametrien yksittäiset tärkeät tiedot, jotta niitä voidaan käyttää ilman, että meillä on UI käytössä
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"></param>
        /// <param name="ownuid"></param>
        /// <returns>{void}</returns>
        public StoredUIComponentParamValues(string kutsuja, long parentuid, long ownuid)
        {
            this.BlockName="";
            this.RouteId=0;
            this.SelectedGroup="";
            this.SelectedOperator="";
            this.PrintValue1="";
            this.PrintValue2="";
        }

        /// <summary>
        /// Tyhjentää koko luokan tiedot täydellisesti, jonka jälkeen luokka ei tiedä enää mihin se on kytketty. Poistetaan myös objectIndexerin listalta
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        public void Clear(string kutsuja, ObjectIndexer objinde)
        {
            string functionname="->(SCP)Clear";
            this.BlockName="";
            this.RouteId=0;
            this.SelectedGroup="";
            this.SelectedOperator="";
            this.PrintValue1="";
            this.PrintValue2="";
            objinde.DeleteObjectFromIndexer(kutsuja+functionname,this.ThisInstanceOwnUID);
            this.ThisInstanceOwnUID=-1;
            this.ParentUID=-1;
        }

        /// <summary>
        /// Pelkästään resetoidaan arvoiltaan tyhjiksi, mutta viittaukset säilyvät muiden objektien UID tietoihin ja komponentti pysyy myös ObjectIndexerin listalla
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        public void Reset(string kutsuja)
        {
            this.BlockName="";
            this.RouteId=0;
            this.SelectedGroup="";
            this.SelectedOperator="";
            this.PrintValue1="";
            this.PrintValue2="";
        }        
 
    }

    /// <summary>
    /// Tämän luokan instanssi nivoo yhteen StoredParamValue luokan tietoelementit sekä itse käyttöliittymän tietoja. Tätä luokkaa joudutaan käyttämään TextBlock komponenttien kanssa, jotta ne saadaan keskustelemaan oikealla tavalla tietomallin ja käyttöliittymän välillä.
    /// </summary>
    public class PrintValueTextBlock
    {
        /// <summary>
        /// Varsinainen TextBlock joka kapsuloidaan tämän luokan sisään
        /// </summary>
        private TextBlock internalTextBlock;

        /// <summary>
        /// Palauttaa varsinaisen TextBlock:in referenssin
        /// </summary>
        public TextBlock ReturnTextBlock {
            get { return this.internalTextBlock; }
        }

        /// <summary>
        /// Property, joka tavallaan ylikirjoittaa kyseisen TextBlockin .Text propertyn
        /// </summary>
        public string Text
        {
            get { return internalTextBlock.Text; }
            set
            {
                if (indeksi==1) {
                    this.StoredParamVals.PrintValue1 = value;  // Tallenna arvo apumuuttujaan.
                } else {
                    this.StoredParamVals.PrintValue2 = value;  // Tallenna arvo apumuuttujaan.
                }
                internalTextBlock.Text = value;    // Aseta arvo alkuperäiseen Text-propertyyn.
            }
        }

        /// <summary>
        /// Tag arvot syötetään long lukuina (UID) suoraan internalTextBlock:in Tag ominaisuuteen
        /// </summary>
        public long Tag {
            get { return (long)this.internalTextBlock.Tag; }
            set { this.internalTextBlock.Tag=value; }
        }

        /// <summary>
        /// Metodi, joka palauttaa tiedon tietomallin sisältämästä kohdetiedosta, eikä itse graafisen komponentin tiedosta
        /// </summary>
        public string BackupText
        {
            get { 
                if (indeksi==1) {
                    return this.StoredParamVals.PrintValue1;
                } else {
                    return this.StoredParamVals.PrintValue2;
                }
            }
        }

        /// <summary>
        /// Referenssi StoredUIComponentParamValues luokan instanssiin, josta apuarvot löytyvät
        /// </summary>
        private StoredUIComponentParamValues StoredParamVals;

        /// <summary>
        /// Indeksi, joka määrittelee mihinkä StoredUIComponentParamValues muuttujaan apuarvo asetetaan
        /// </summary>
        private int indeksi=0;

        /// <summary>
        /// Enumeraatio joka määrittelee, mihin kohteeseen tiedot menevät 
        /// </summary>
        public enum textBlockType {
            PRINT_VALUE_1=1,
            PRINT_VALUE_2=2
        };

        /// <summary>
        /// Constructor luokalle, joka nivoo yhteen StoredParamValue luokan tietoelementit sekä itse käyttöliittymän tietoja. Tätä luokkaa joudutaan käyttämään TextBlock komponenttien kanssa, jotta ne saadaan keskustelemaan oikealla tavalla tietomallin ja käyttöliittymän välillä.
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="stouiparam"></param>
        /// <param name="existingtb">TextBlock, referenssi TextBlockista joka halutaan asettaa sisään tähän luokkaan</param>
        /// <param name="indexi">enum textBlockType, määrittelee mikä komponentti on kyseessä, jota luokalla käytetään ja manipuloidaan </param>
        /// <returns> {void} </returns>
        public PrintValueTextBlock(string kutsuja, StoredUIComponentParamValues stouiparam, TextBlock existingtb, int indexi=1)
        {
            this.StoredParamVals=stouiparam;
            this.indeksi=indexi;
            this.internalTextBlock=existingtb;
        }
    }

    /// <summary> Tämän luokan instanssit ylläpitävät pelkästään Action Centren objektien käyttöliittymän (UI) komponentteja </summary>
    public class StoredUIObjectsForActionCentre
    {
        /// <summary> Referenssi TextBox:iin, josta löytyy blokin nimi </summary>
        public TextBox blockname;

        /// <summary> Referenssi TextBox:iin, josta löytyy RouteId, eli käytännössä sama tieto kuin Slotlistin AltRoute </summary>
        public TextBox routeid;
        
        /// <summary>
        /// Referenssi ComboBox:iin, josta löytyy valittu ryhmä operaattoricomboboxille
        /// </summary>
        public ComboBox SelectedGroupCombo;

        /// <summary> Referenssi ComboBox:iin, josta löytyy valittu operaatori, varsinainen kohde jne. </summary> 
        public ComboBox SelectedHandle;

        /// <summary> Referenssi TextBlock:iin, johon voimme printata jotain näkymään itse blockiin - valuearea 1 </summary>
        public PrintValueTextBlock printvalue1;

        /// <summary> Referenssi TextBlock:iin, johon voimme printata jotain näkymään itse blockiin - valuearea 2 </summary>
        public PrintValueTextBlock printvalue2;

        /// <summary> Referenssi infopaneliin, jossa on kaikki tekstitiedot itse varsinaisesta MotherConnectionRectanglesta </summary>
        private StackPanel infopanel;
        /// <summary> Referenssi infopaneliin, jossa on kaikki tekstitiedot itse varsinaisesta MotherConnectionRectanglesta </summary>
        public StackPanel InfoPanel {
            get { return this.infopanel; }
            set { this.infopanel=value; }
        }

        private long localparentuid=-1;
        /// <summary> Laatikon oma (MotherRectanglen) uniqrefnum </summary>
        public long ParentUID { 
            get { return this.localparentuid; }
            set {
                this.localparentuid=value;
                this.StoredParamValues.GraphCompOverParentUID=value;
            } 
        }

        private long localloadedparentuid=-1;
        /// <summary>
        /// Latauksen yhteydessä ollut laatikon oma (MotherRectanglen) uniqrefnum
        /// </summary>
        public long LoadedParentUID { 
            get { return this.localloadedparentuid; } 
            set {
                this.localloadedparentuid=value;
                this.StoredParamValues.GraphCompLoadedOverParentUID=value;
            } 
        }

        private long localthisinstanceuid=-1;
        /// <summary>
        /// Tämän objektin oma UID. Tätä UID arvoa ei kuitenkaan käytetä varsinaisesti kuin parametriarvojen rekisteröinnissä StoredUIComponentParamValues luokalle, koska itse graafisten komponenttien osalta äitiobjektiksi merkataan suoraan (MotherConnectionRectange)
        /// </summary>
        public long ThisInstanceOwnUID { 
            get { return this.localthisinstanceuid; } 
            set {
                this.localthisinstanceuid=value;
                this.StoredParamValues.GraphCompParentUID=value;
            }
        }
        
        private bool registeredobject=false;
        /// <summary>
        /// Tämä property kertoo, onko objekti rekisteröity oikealla tavalla. Tämä tieto tarvitaan sen oikeaa poistamista varten.
        /// </summary>
        public bool RegisteredObject {
            get { return this.registeredobject; }
        }

        /// <summary> Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </summary>
        private ObjectIndexer objectindexerref;

        /// <summary> Käyttöliittymäluokan referenssi </summary>
        private ProgramHMI programhmi;        

        /// <summary>
        /// Tämä luokan instanssi hoitaa useiden luokkien kohdalla parametrien printtauksen yhdellä vakioidulla tavalla
        /// </summary>
        private ParamPrinter paramprintterit;

        /// <summary>
        /// Graafisten komponenttien parametrien arvojen säilyttämiselle oman säilytysluokan instanssin referenssi. Tällä voidaan luoda yhteydet komponenttien välille, ilman että ne ovat kiinnitettyinä graafisten komponentin tietoihin
        /// </summary>
        public StoredUIComponentParamValues StoredParamValues;

        /// <summary> int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </summary>
        private int createuicomps=0;

        /// <summary> Constructor luokalle, joka vastaa äitilaatikoiden graafisten komponenttien säilyttämisestä </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentid"> long, motherrectanglen uniqrefnum </param>
        /// <param name="proghr"> ProgramHMI, referenssi käyttöliittymäluokkaan </param>
        /// <param name="parprinte"> ParamPrinter, referenssi parametri printteri luokan instanssiin, jolla voidaan printata parametreja usealla vakioidulla tavalla </param>
        /// <param name="objin"> ObjectIndexer, Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </param>
        /// <param name="creategraphcomp"> int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </param>
        /// <returns> {void} </returns>
        public StoredUIObjectsForActionCentre(string kutsuja, long parentid, ProgramHMI proghr, ParamPrinter paramprinte, ObjectIndexer objin, int creategraphcomp)
        {
            string functionname="->(SUIO)Constructor";
            this.ParentUID=parentid;
            this.programhmi=proghr;
            this.objectindexerref=objin;
            this.createuicomps=creategraphcomp;

            this.ThisInstanceOwnUID=this.objectindexerref.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.MOTHER_COMPONENT_STOREDUICOMPONENT_103,-1);
            if (this.ThisInstanceOwnUID<0) {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+this.ThisInstanceOwnUID+" ParentUid:"+parentid,-1063,4,4);
            }

            //this.paramprintterit=paramprinte;
            this.paramprintterit = new ParamPrinter(kutsuja+functionname,this.programhmi,paramprinte.merkkijonokasittelija);

            long storedparamsuid = this.objectindexerref.AddObjectToIndexer(kutsuja+functionname,this.ThisInstanceOwnUID,(int)ObjectIndexer.indexerObjectTypes.STOREDUI_PARAM_VALUES_150,-1);
            if (storedparamsuid>=0) {
                this.StoredParamValues = new StoredUIComponentParamValues(kutsuja+functionname, this.ThisInstanceOwnUID, storedparamsuid); // Luodaan graafisten komponenttien parametrien säilyttämiselle oman säilytysluokan instanssi
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+storedparamsuid+" ParentUid:"+parentid,-1064,4,4);
                this.StoredParamValues=null;
            }
        }

        /// <summary>
        /// Tämä metodi päivittää InfoPanel:in paikan Canvasilla komponenttia siirettäessä
        /// </summary>
        /// <param name="kutsuja"></param>
        /// <param name="newTop">double, uusi paikka ylhäältä laskettuna, johon kohde siirretään </param>
        /// <param name="newLeft">double, uusi paikka vasemmalta laskettuna, johon kohde siirretään </param>
        /// <returns>{int} Palauttaa 1, jos kohteen siirto onnistui. Jos virhe, palauttaa pienemmän luvun kuin 0. </returns>
        public int UpdateInfoPanelDuringDrag(string kutsuja, double newTop, double newLeft)
        {
            int retVal=-1;

            if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                // Siirretään Infopanelia
                Canvas.SetLeft(this.InfoPanel, newLeft);
                Canvas.SetTop(this.InfoPanel, newTop);
                retVal=1;
            } else {
                retVal=1; // Asetetaan luku 1 palautettavaksi, jos ei mitään kaikesta huolimatta päivitetäkkään
            }
            return retVal;
        }
      
        /// <summary>
        /// Tämä metodi palauttaa JSON muodossa tähän objektiin tallennetun datan tallennusta varten
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {string} Palauttaa tämän objektin sisällön JSON muodossa tallennettavaksi </returns>
        public string ReturnStoredDataAsJSONForSaving(string kutsuja)
        {
            string functionname="->(SUIO)ReturnStoredDataAsJSON";
            string retVal="";
            this.paramprintterit.SetStoredUIObjectsToPrint(this);
            retVal=this.ReturnStoredDataAsJSON(kutsuja+functionname,ParamNameLists.storedUIobjSavingParameterNames);
            return retVal;
        }

        /// <summary>
        /// Tämä metodi palauttaa JSON muodossa tähän objektiin tallennetun datan
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="storeduiobjsparamnames">List string olion referenssi, jossa on parametrit, joilla halutaan kerätä printattavat kohteet tämän objektin instanssin tiedoista</param>
        /// <returns> {string} Palauttaa tämän objektin sisällön JSON muodossa tallennettavaksi </returns>
        public string ReturnStoredDataAsJSON(string kutsuja, List<string> storeduiobjsparamnames)
        {
            string functionname="->(SUIO)ReturnStoredDataAsJSON";
            string retVal="";
            if (storeduiobjsparamnames!=null) {
                if (storeduiobjsparamnames.Count>0) {            
                    this.paramprintterit.SetStoredUIObjectsToPrint(this);
                    retVal=this.paramprintterit.MyOwnParamPrint(kutsuja+functionname,storeduiobjsparamnames,(int)ParamPrinter.myOwnTypePrintingEnum.JSON_OBJECT_WITH_PARAM_NAMES_AND_VALUES_2);
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"Parameter list didn't contain any parameter name (JSONparameters)!",-891,4,4);
                    retVal="ERROR=-110";
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Parameter list was null (JSONparameters)!",-892,4,4);
                retVal="ERROR=-111";
            }            
            return retVal;
        }

        /// <summary>
        /// Tämä metodi muuttaa tietoja myös StoredParamValues objektissa, aina kun tiedot muuttuvat itse graafisessa komponentissa nimeltä: routeid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns> {void} </returns>
        private void RouteId_LostFocus(object sender, RoutedEventArgs e)
        {
            string functionname="->(SUIO)RouteId_LostFocus";
            int routei=0;
            // Kirjoita tähän koodi, joka suoritetaan, kun käyttäjä poistuu TextBox-komponentista.
            if (this.routeid != null && this.StoredParamValues != null)
            {
                if (this.routeid.Text!="") {
                    bool succ=this.programhmi.TestValueType(functionname,"N",this.routeid.Text);
                    if (succ==true) {
                        bool succe=int.TryParse(this.routeid.Text, out routei);
                        if (succe==true) {
                            this.StoredParamValues.RouteId = routei;
                        } else {
                            this.routeid.Text="-3";
                        }
                    } else {
                        this.routeid.Text="-1";
                    }
                } else {
                    this.routeid.Text="-2";
                }
            }            
        }

        /// <summary>
        /// Tämä metodi muuttaa tietoja myös StoredParamValues objektissa, aina kun tiedot muuttuvat itse graafisessa komponentissa nimeltä: blockname
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns> {void} </returns>
        private void Blockname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.blockname != null && this.StoredParamValues != null)
            {
                this.StoredParamValues.BlockName = this.blockname.Text;
            }
        }                

        /// <summary> Tämä metodi rekisteröi ne käyttöliittymäkomponentit, joita tämä luokka pitää sisällään ja käyttää </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="blockn"> TextBox, komponentti, josta löytyy blokin nimi </param>
        /// <param name="route"> TextBox, komponentti josta löytyy RouteId </param>
        /// <param name="grouph"> Combobox, komponentti johon on talletettu blokissa ryhmän valinta </param>
        /// <param name="valueh"> Combobox, komponentti johon on talletettu blokissa tehty valinta </param>
        /// <param name="printval1"> TextBlock, komponentti johon voi printata jotain rivi 1 </param> 
        /// <param name="printval2"> TextBlock, komponentti johon voi printata jotain rivi 2 </param>       
        /// <returns> {void} </returns>   
        public void RegisterUIComponents(string kutsuja, TextBox blockn, TextBox route, ComboBox grouph, ComboBox valueh, TextBlock printval1, TextBlock printval2)
        {
            string functionname="->(SUIO)RegisterUIComponents";
            long thiscomponentownUID=-1;

            this.blockname=blockn;
            thiscomponentownUID=this.objectindexerref.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.UI_COMPONENT_TEXTBOX_201,-1); // Tässä ParentUID tarkoittaa itse Motherobjektin UID:ia, eli käytännössä tämän objektin parentia
            if (thiscomponentownUID>=0) {
                this.blockname.Tag=thiscomponentownUID; // Asetetaan saatu tieto komponentin Tag tiedoksi, jonka jälkeen komponentin Tag tietoa käyttämällä saamme luettua komponentin tyypin sen parentin objectindexeriltä
                this.blockname.TextChanged += Blockname_TextChanged;
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+thiscomponentownUID+" ParentUid:"+this.ParentUID,-1065,4,4);
            }

            this.routeid=route;
            thiscomponentownUID=this.objectindexerref.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.UI_COMPONENT_TEXTBOX_201,-1); // Tässä ParentUID tarkoittaa itse Motherobjektin UID:ia, eli käytännössä tämän objektin parentia
            if (thiscomponentownUID>=0) {
                this.routeid.Tag=thiscomponentownUID;
                this.routeid.LostFocus += RouteId_LostFocus;
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+thiscomponentownUID+" ParentUid:"+this.ParentUID,-1066,4,4);
            }                

            this.SelectedGroupCombo=grouph;
            thiscomponentownUID=this.objectindexerref.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.UI_COMPONENT_COMBOBOX_202,-1); // Tässä ParentUID tarkoittaa itse Motherobjektin UID:ia, eli käytännössä tämän objektin parentia
            if (thiscomponentownUID>=0) {
                this.SelectedGroupCombo.Tag=thiscomponentownUID;        
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+thiscomponentownUID+" ParentUid:"+this.ParentUID,-1067,4,4);
            }

            this.SelectedHandle=valueh;
            thiscomponentownUID=this.objectindexerref.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.UI_COMPONENT_COMBOBOX_202,-1); // Tässä ParentUID tarkoittaa itse Motherobjektin UID:ia, eli käytännössä tämän objektin parentia
            if (thiscomponentownUID>=0) {    
                this.SelectedHandle.Tag=thiscomponentownUID;
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+thiscomponentownUID+" ParentUid:"+this.ParentUID,-1068,4,4);
            }                

            this.printvalue1=new PrintValueTextBlock(kutsuja+functionname,this.StoredParamValues,printval1,(int)PrintValueTextBlock.textBlockType.PRINT_VALUE_1);
            thiscomponentownUID=this.objectindexerref.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.UI_COMPONENT_TEXTBLOCK_200,-1); // Tässä ParentUID tarkoittaa itse Motherobjektin UID:ia, eli käytännössä tämän objektin parentia
            if (thiscomponentownUID>=0) {    
                this.printvalue1.Tag=thiscomponentownUID;
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+thiscomponentownUID+" ParentUid:"+this.ParentUID,-1069,4,4);
            }                

            this.printvalue2=new PrintValueTextBlock(kutsuja+functionname,this.StoredParamValues,printval2,(int)PrintValueTextBlock.textBlockType.PRINT_VALUE_2);
            thiscomponentownUID=this.objectindexerref.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.UI_COMPONENT_TEXTBLOCK_200,-1); // Tässä ParentUID tarkoittaa itse Motherobjektin UID:ia, eli käytännössä tämän objektin parentia
            if (thiscomponentownUID>=0) {    
                this.printvalue2.Tag=thiscomponentownUID;
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+thiscomponentownUID+" ParentUid:"+this.ParentUID,-1070,4,4);
            }

            this.registeredobject=true; // Kohde on rekisteröity oikealla tavalla, jotta sen Remove metodia voidaan kutsua.
        }

        /// <summary>
        /// Tämä metodi yrittää asettaa SelectedHandle Comboboxiin ja SelectedOperator muuttujaan selectedHandle tiedon
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="selectedGroup"> string, nimike jota yritetään asettaa SelectedHandle Comboboxiin ja SelectedOperator muuttujaan </param>
        /// <param name="needtosetgroupcombo">bool, jos kyllä, niin yritetään setata SelectedHandle Comboboxiin selectedHandle kohde. Jos false, niin näin ei tehdä </param>
        /// <returns> {void} </returns>        
        public void SetSelectedCombo(string kutsuja, string selectedHandle, bool needtosetselectedhandle=true)
        {
            string functionname="->(SUIO)SetGroupCombo";

            if (this.registeredobject==true) {
                if (needtosetselectedhandle==true) {
                    // Asetetaan valittu operaattori kohteelle tiedoksi
                    foreach (var item in this.SelectedHandle.Items) { // Käy läpi kaikki kohteet ComboBoxissa
                        if (item.ToString() == selectedHandle) { // Oletetaan, että kohteen voi muuttaa merkkijonoksi
                            this.SelectedHandle.SelectedItem = item;
                            break;
                        }
                    }
                }

                this.StoredParamValues.SelectedOperator=selectedHandle;

            } else {
                this.programhmi.sendError(kutsuja+functionname,"This class was not registered correctly!",-944,4,4);
            }
        }

        /// <summary>
        /// Tämä metodi yrittää asettaa SelectedGroupCombo Comboboxiin ja SelectedGroup muuttujaan selectedGroup tiedon
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="selectedGroup"> string, nimike jota yritetään asettaa SelectedGroupCombo Comboboxiin ja SelectedGroup muuttujaan </param>
        /// <param name="needtosetgroupcombo">bool, jos kyllä, niin yritetään setata SelectedGroupCombo Comboboxiin selectedGroup kohde. Jos false, niin näin ei tehdä </param>
        /// <returns> {void} </returns>
        public void SetGroupCombo(string kutsuja, string selectedGroup, bool needtosetgroupcombo=true)
        {
            string functionname="->(SUIO)SetGroupCombo";

            if (this.registeredobject==true) {
                if (selectedGroup!="") {
                    if (needtosetgroupcombo==true) {
                        // Asetetaan valittu operaattori kohteelle tiedoksi
                        foreach (var item in this.SelectedGroupCombo.Items) { // Käy läpi kaikki kohteet ComboBoxissa
                            if (item.ToString() == selectedGroup) { // Oletetaan, että kohteen voi muuttaa merkkijonoksi
                                this.SelectedGroupCombo.SelectedItem = item;
                                break;
                            }
                        }
                    }

                    this.StoredParamValues.SelectedGroup=selectedGroup;
                    this.SelectedHandle.Items.Clear();

                    switch (selectedGroup)
                    {
                        case "SLOT_VALUES":
                            foreach(string valuname in SlotList.slotsprintparams) {
                                SelectedHandle.Items.Add(valuname);
                            }
                            break;
                        case "MAIN_PARAMS":
                            foreach(string valuname in SmartBot.mainParamNames) {
                                SelectedHandle.Items.Add(valuname);
                            }
                            break;
                        case "TRIGGERLIST_PARAMS":
                            foreach(string valuname in SmartBot.triggerlistParamNames) {
                                SelectedHandle.Items.Add(valuname);
                            }
                            break;                                        
                        case "COURSE_INFO":
                            foreach(string valuname in SmartBot.courseInfoNames) {
                                SelectedHandle.Items.Add(valuname);
                            }
                            break;                                        
                        default:
                            this.programhmi.sendError(functionname,"No such selected group name! Group name:"+selectedGroup,-946,4,4);
                            break;
                    } 
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"SelectedGroup was empty!",-947,2,4);
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"This class was not registered correctly!",-945,4,4);
            }
        }

        /// <summary>
        /// Tämä metodi auttaa poistamaan objekti tuhottaessa myös muilta listoilta, joihin objektit on rekisteröity 
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="origcanvas"> Canvas, se canvas josta kyseinen infopanel poistetaan</param>
        /// <returns> {void} </returns>
        public void RemoveStoredUIObjects(string kutsuja, Canvas origcanvas)
        {
            string functionname="->(SUIO)RemoveStoredUIObject";
            long removenum=-1;
            int resp=-1;

            int numberofuicomponents=6; // Kaikkien graafisten komponenttien määrä joita säilytetään

            for (int i=0; i<numberofuicomponents; i++) {
                switch (i)
                {
                    case 0:
                        removenum=(long)this.blockname.Tag;
                        break;
                    case 1:
                        removenum=(long)this.routeid.Tag;
                        break;
                    case 2:
                        removenum=(long)this.SelectedHandle.Tag;
                        break;
                    case 3:
                        removenum=this.printvalue1.Tag;
                        break;                                                                                            
                    case 4:
                        removenum=this.printvalue2.Tag;
                        break;
                    case 5:
                        removenum=(long)this.SelectedGroupCombo.Tag;
                        break;                        
                    default:
                        removenum=-5;
                        break;
                }

                if (removenum>=0) {
                    resp=this.objectindexerref.DeleteObjectFromIndexer(kutsuja+functionname,removenum);
                    if (resp<1) {
                        this.programhmi.sendError(kutsuja+functionname,"Error during removing objectindexer object! i="+i+" Removenum: "+removenum+" Response:"+resp,-740,4,4); 
                    }
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"Error during removing objectindexer object! i="+i+" Removenum: "+removenum,-761,4,4);
                }                
            }

            // Poistetaan komponentit niitä sisältävistä kontrolleista
            if (infopanel != null)
            {
                if (blockname != null) infopanel.Children.Remove(blockname);
                if (routeid != null) infopanel.Children.Remove(routeid);
                if (SelectedHandle != null) infopanel.Children.Remove(SelectedHandle);
                try
                {
                    if (SelectedGroupCombo != null) infopanel.Children.Remove(SelectedGroupCombo);
                    if (printvalue1 != null) infopanel.Children.Remove(this.printvalue1.ReturnTextBlock);
                    if (printvalue2 != null) infopanel.Children.Remove(this.printvalue2.ReturnTextBlock);
                }
                catch (Exception ex)
                {
                    // Tarkoituksella tyhjä, ettei heitä virheitä kun infopaneliin liittymättömiä komponentteja toisinaan yritetään tuhota
                }
            }

            // Poistetaan viittaukset
            blockname = null;
            routeid = null;
            this.StoredParamValues.SelectedGroup = null;
            SelectedHandle = null;
            printvalue1 = null;
            printvalue2 = null;
            this.StoredParamValues.Clear(kutsuja+functionname,this.objectindexerref);
            this.StoredParamValues=null;  

            if (infopanel!=null) {
                if (origcanvas!=null) {
                    origcanvas.Children.Remove(infopanel);
                }
            }
        }       
    }