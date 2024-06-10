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

    /// <summary> Tämä luokka pitää sisällään listat ja metodit, joita tarvitaan ylläpitämään kaikki Connection objektit järjestyksessä </summary>
    public class ConnectionsHandler
    {
        /// <summary> Tämän ConnectionHandlerin oma uniqrefnum </summary>
        public long OwnUID { get; set; }

        /// <summary>
        /// Tämän ConnectionHandlerin vanhemman UID
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Tämän ConnectionHandlerin isovanhemman UID
        /// </summary>
        public long GranParentUID { get; set; }

        /// <summary> Lista Connection luokan instansseja, jotka yhdistävät kaksi laatikkoa viivalla toisiinsa. Avaimena toimii connectionin uid </summary>
        public SortedList<long, Connection> connections;

        /// <summary> Käyttöliittymäluokan referenssi </summary> 
        private ProgramHMI prohmi;
    
        /// <summary> Referenssi ObjectIndexer luokka </summary>
        private ObjectIndexer objindexerobj;

        /// <summary> PriorityFileSaver luokan referenssi rekisteröidään connectionshandlerille, jotta tapahtumia voidaan tallettaa kovalevylle siten, että luokka priorisoi tärkeimmät talletettavat ja tallentaa kohteita kiireellisyysjärjestyksessä </summary>
        private PriorityFileSaver pfsaver;

        /// <summary>
        /// ParamPrinter luokan referenssi, jolla voidaan printata mm. Connection objektien tietoja eri muodoissa tallennusta silmällä pitäen
        /// </summary>
        private ParamPrinter paramprintteri;

        /// <summary>
        /// Muuttuja, joka kuvaa miten luokan initialisointi on edistynyt
        /// </summary>
        private int isclassokay=-1;

        /// <summary>
        /// Muuttuja, joka kertoo mitä isclassokay muuttujan täytyy olla, jotta luokka olisi oikein initialisoitu
        /// </summary>
        private int classokaytreshold=1;

        /// <summary>
        /// Tämä property kertoo, onko luokka initialisoitinsa puolesta kunnossa vai ei ja onko luokan funktiot käytettävissä normaalisti
        /// </summary> 
        public bool IsClassInitialized {
            get {
                if (this.isclassokay>=classokaytreshold) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        /// <summary>
        /// Kansion nimi, jonne connections tiedostot tallennetaan
        /// </summary>
        public static string ConnectionsSavingFolderName="actions";

        /// <summary>
        /// Tiedostonimen perusosa connections blokeille, joilla kyseisiä tiedostoja tallennetaan kovalevylle
        /// </summary>
        public static string ConnectionsBlockBasicName="connectionlist";        

        /// <summary>
        /// Kaikki Connection tyyppisten objektien parametrit. Tällä listalla voidaan esim. printata kaikki Connectionin parametrien tiedot vaikka JSON objektissa ja tallentaa objekti kovalevylle.
        /// </summary>
        public static List<string> allConnectionParameterNames = new List<string>
        {
            "ThisConnectionOwnUID",
            "Box1Tag",
            "Box1MotherBoxUID",
            "Box1OwnUID",
            "Box2Tag",
            "Box2MotherBoxUID",
            "Box2OwnUID",
            "ConnectionLineTag",
            "EndPointCircleTag",
            "InfoTextTag",
            "ConnectionLineStroke"
        };

        /// <summary>
        /// Kohteen tallennuksessa tarvittavat Connection objektin parametrit
        /// </summary>
        public static List<string> allSavingConnectionParameterNames = new List<string>
        {
            "ThisConnectionOwnUID",
            "Box1OwnUID",
            "Box2OwnUID",
            "ConnectionLineStroke"
        };        

        /// <summary>
        /// Tiedostojen loppupääte, jolla päätteellä tiedostot talletetaan ja luetaan kovalevyltä 
        /// </summary>
        private string fileexten=".txt";

        /// <summary>
        /// Luodaanko UI:n graafiset komponentit samalla
        /// </summary>
        private int createuicomps;

        /// <summary> Luokan constructor, joka hoitaa connection instanssien pitämisestä järjestyksessä listassa </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"> long, äitiobjektin UID </param>
        /// <param name="granparentuid"> long, äitiobjektin vanhemman UID </param>        
        /// <param name="progh"> ProgramHMI, referenssi käyttöliittymäluokkaan </param>         
        /// <param name="objindexe"> ObjectIndexer, referenssi ObjectIndexer luokkaan </param>
        /// <param name="file_exte"> string, Tiedostojen loppupääte, jolla päätteellä tiedostot talletetaan ja luetaan kovalevyltä </param>
        /// <param name="creategraphcomp">int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </param>
        /// <returns> {void} </returns>
        public ConnectionsHandler(string kutsuja, long parentuid, long granparentuid, ProgramHMI progh, ObjectIndexer objindexe, string file_exte, int creategraphcomp) {
            string functionname="->(CSH)ConnectionsHandler";
            this.connections = new SortedList<long, Connection>();
            this.prohmi=progh;
            this.objindexerobj=objindexe;
            this.fileexten=file_exte;
            this.createuicomps=creategraphcomp;

            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;

            this.OwnUID = this.objindexerobj.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.ACTIONCENTREUI_CLASS_INSTANCE_300,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.GranParentUID); // Tässä jälkimmäinen -1 on objectindexarray            

            if (this.OwnUID>=0) {
                int respo = this.objindexerobj.SetObjectToIndexerWithErrorReport(kutsuja + functionname, this.OwnUID, this, (int)ObjectIndexer.rewriteOldObjectReference.ALWAYS_REWRITE_OBJECT_REFERENCE_0);
                if (respo >= 0) {
                    this.isclassokay++;
                } else {
                    this.prohmi.sendError(kutsuja + functionname, "Failed to set object to indexer with error report. Response: " + respo, -1268, 4, 4);
                }                    
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+this.OwnUID+" ParentUid:"+this.ParentUID,-1269,4,4);
                this.isclassokay=-5;
            }
        }

        /// <summary>
        /// Tämä metodi palauttaa Connection objektin referenssin, vaikka se ei kuuluisi tämän listan kohteisiin, jos kohde on vain rekisteröity täältä löytyvälle listalle
        /// </summary>
        /// <param name="kutsuja">string, The caller's path.</param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä boxlist listan listoilta</param>
        /// <returns>{Connection} Palauttaa Connection objektin referenssin, jos sellainen löytyy UIDtoseek muuttujalla ja null, jos tuli virhe joko etsinnässä. </returns>
        public Connection GetConnectionByUID(string kutsuja, long UIDtoseek)
        {
            string functionname="->(CSH)GetConnectionByUID";
            
            if (this.IsClassInitialized==true) {
                if (this.connections.IndexOfKey(UIDtoseek)>-1) {
                    return this.connections[UIDtoseek];
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Error with seeking object reference from connections list! UID:"+UIDtoseek,-1173,4,4);
                    return null;
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.isclassokay+" when treshold:"+this.classokaytreshold,-1172,4,4);
                return null;
            }              
        }        

        /// <summary> Luokan Init funktio, joka asettaa referenssejä luokan luomisen jälkeen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="pfisaver"> PriorityFileSaver luokan referenssi rekisteröidään slottilistoille, jotta tapahtumia voidaan tallettaa kovalevylle siten, että luokka priorisoi tärkeimmät talletettavat ja tallentaa kohteita kiireellisyysjärjestyksessä </param>
        /// <param name="parprint"> ParamPrinter luokan referenssi, jotta saadaan parametrilistoilla printattua haluamiamme tietoja objekteista </param>
        /// <returns> {void} </returns>
        public void InitConnectionsHandler(string kutsuja, PriorityFileSaver pfisaver, ParamPrinter parprint) 
        {
            this.pfsaver=pfisaver;
            this.paramprintteri=parprint;
            this.isclassokay++;
        }

        /// <summary>
        /// Tämä metodi tallentaa kaikki connectionit ja niiden tiedot. Tämä metodin kutsuminen ei kuitenkaan ole aina järkevää, koska connection tiedot kannattaa toisinaan koostaa osaksi suurempaa tiedostoa, jota varten kannattaa käyttää PrintAllConnections käskyä
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="name_ext"> string, tallennetun tiedoston tiedostonimen lisätieto, jolla tiedostonimi saadaan yksilölliseksi </param>
        /// <returns> {int} Palauttaa 1, jos tietojen lähettäminen tallennukseen onnistui. Jos palauttaa pienempi kuin 0, niin kyseessä oli virhe! </returns>
        public int SaveAllConnections(string kutsuja,string name_ext)
        {
            string functionname="->(CSH)SaveAllConnections";
            int retVal=-1;
            string savestring="";

            if (this.IsClassInitialized==true) {
                savestring=this.PrintAllConnections(kutsuja+functionname,ConnectionsHandler.allSavingConnectionParameterNames,(int)ParamPrinter.myOwnTypePrintingEnum.JSON_OBJECT_WITH_PARAM_NAMES_AND_VALUES_2);
                if (savestring!="") {
                    this.pfsaver.AddFileSaveItem(kutsuja+functionname,ConnectionsSavingFolderName,ConnectionsBlockBasicName+"_"+name_ext+this.fileexten,savestring,2000000,true);
                    retVal=1;
                } else {
                    retVal=-3;
                    this.prohmi.sendError(kutsuja+functionname,"Saving string was empty!",-870,4,4);
                }
            } else {
                retVal=-2;
                this.prohmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassokay,-785,4,4);
            }
            return retVal;  
        }

        /// <summary>
        /// Palauttaa string muotoisen merkkijonon, johon on printattu halutut parametrit kaikista connectioneista
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="paramnamelist"> List string muotoinen lista parametrien nimistä, joita halutaan palautettavan </param>
        /// <param name="printingtype">int, If 0, prints parameters with their names, separated by tabs. If 1, prints just values separated by commas. If 2, prints parameters and their values as JSON object.</param>
        /// <returns> {string} Palauttaa merkkijonon, jossa eri connectionit on eroteltu omille riveilleen printtauksen yhteydessä. Tämä on soveltuvaa, jos printtaamme kaikki connectionit kerralla esim. JSON muodossa </returns>
        public string PrintAllConnections(string kutsuja, List<string> paramnamelist, int printingtype)
        {
            string functionname="->(CSH)PrintAllConnections";
            string retVal="";
            long keyval=-1;

            if (this.IsClassInitialized==true) {
            
                int amo=this.connections.Count();

                if (amo>0) {
                    retVal=retVal+"{"+Convert.ToChar(13);
                    for (int i=0; i<amo; i++) {
                        keyval=this.connections.ElementAt(i).Key;
                        retVal=retVal+"\""+keyval+"\":\"";
                        retVal=retVal+this.PrintConnection(kutsuja+functionname,keyval,paramnamelist,printingtype);
                        if (i<amo-1) {
                            retVal=retVal+"\","+Convert.ToChar(13);
                        } else {
                            retVal=retVal+"\"";
                        }
                    }
                    retVal=retVal+Convert.ToChar(13)+"}";
                    retVal=ParseJSON.IntendSerializedJSON(kutsuja+functionname,retVal);
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"No connections in list! Return empty string!",-783,2,4);
                    retVal="";
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassokay,-784,4,4);
            }            

            return retVal;            
        }

        /// <summary>
        /// Tämä funktio palauttaa yhden connectionin halutut tiedot printtausta tai tallennusta varten
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="connectionuid"> long, connections sortedlistan kohteen uid, jonka tietoja halutaan palautettavan string muotoisena </param>
        /// <param name="paramnamelist"> List string muotoinen lista parametrien nimistä, joita halutaan palautettavan </param>
        /// <param name="printingtype">If 0, prints parameters with their names, separated by tabs. If 1, prints just values separated by commas. If 2, prints parameters and their values as JSON object.</param>
        /// <returns> A single string containing the specified order parameters and their values in requested format. Palauttaa tyhjän stringin, jos virhe! </returns>
        public string PrintConnection(string kutsuja, long connectionuid, List<string> paramnamelist, int printingtype)
        {
            string functionname="->(CSH)PrintConnections";
            string retVal="";

            if (this.IsClassInitialized==true) {
            
                int amo=this.connections.Count();

                if (amo>0) {
                    if (this.connections.IndexOfKey(connectionuid)>-1) {
                        this.paramprintteri.SetConnectionObjectToPrint(this.connections[connectionuid]); // Asetetaan haluttu connection objekti, jotta voidaan sen jälkeen printata sen tiedot
                        retVal=this.paramprintteri.MyOwnParamPrint(kutsuja+functionname,paramnamelist,printingtype);
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"No such connection with UID:"+connectionuid,-782,4,4);
                        retVal=""; 
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"No connections in list! Return empty string!",-780,2,4);
                    retVal="";
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassokay,-781,4,4);
            }            

            return retVal;
        }

        /// <summary>
        /// Tämä metodi poistaa yhden connectionin connections listalta sekä siihen liittyvät komponentit. Lisäksi tämä metodi poistaa viitteet niihin laatikoihin, joihin tämä kohde on ollut yhdistettynä
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="removingconnectionuid">long, sen Connection kohteen key arvo, joka ollaan poistamassa connections listalta</param>
        /// <param name="origcanvas">Canvas, se canvas, josta connectionin graafiset komponentit poistetaan</param>
        /// <param name="listofallboxesreferences">SortedList long, ConnectionRectangle, referenssi listaan, joka pitää kirjaa kaikista suorakulmioista jotka canvasille on piirretty </param>
        /// <returns> {void} </returns>
        public void RemoveSingleConnection(string kutsuja, long removingconnectionuid, Canvas origcanvas, SortedList<long, ConnectionRectangle> listofallboxesreferences)
        {
            string functionname="->(CSH)RemoveSingleConnection";
            int answ1=-1;
            int answ2=-1;

            if (this.IsClassInitialized==true) {

                long ownuid=removingconnectionuid;

                long box1uid=this.connections[ownuid].Box1OwnUID;
                long box2uid=this.connections[ownuid].Box2OwnUID;

                ConnectionRectangle tempconrect;

                // Poistetaan ensin connectioneiden UID tiedot itse pikkulaatikoilta, jotka on yhdistetty toisiinsa
                if (listofallboxesreferences.IndexOfKey(box1uid)>-1) {
                    tempconrect = listofallboxesreferences[box1uid];
                    answ1=tempconrect.RemoveConnectionUID(kutsuja+functionname,ownuid);
                    if (answ1<1) {
                        this.prohmi.sendError(kutsuja+functionname,"Removing object wasn't in ConnectionsUID list! Response:"+answ1+" RemovingUID:"+ownuid,-744,4,4);
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Unable to remove object uid from listofallboxes! Box1 UID:"+box1uid,-745,4,4);
                }

                if (listofallboxesreferences.IndexOfKey(box2uid)>-1) {
                    tempconrect = listofallboxesreferences[box2uid];
                    answ2=tempconrect.RemoveConnectionUID(kutsuja+functionname,ownuid);
                    if (answ2<1) {
                        this.prohmi.sendError(kutsuja+functionname,"Removing object wasn't in ConnectionsUID list! Response:"+answ2+" RemovingUID:"+ownuid,-746,4,4);
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Unable to remove object uid from listofallboxes! Box2 UID:"+box2uid,-747,4,4);
                }     

                if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                    if (origcanvas!=null) {
                        this.connections[ownuid].RemoveConnectionComponents(kutsuja+functionname,origcanvas); // Kohde poistetaan piirrettävältä ruudulta
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Canvas was null, althought UI creation was on:"+this.createuicomps,-1047,4,4);
                    }
                }
                
                this.connections.Remove(ownuid); // Sitten poistetaan itse kohde Connections listalta sen key arvolla
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassokay,-776,4,4);
            }
        }

        /// <summary> Yhteyden luonti metodi kahden laatikon välille </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="origcanvas"> Canvas, referenssi siihen canvasiin, jonka sisään yhdysviivat luodaan </param>
        /// <param name="box1ownuid"> long, referenssi siihen suorakulmioon uid:iin, joka halutaan yhdistää </param>
        /// <param name="box2ownuid"> long, referenssi siihen suorakulmioon uid:iin, johon halutaan yhdistää </param>
        /// <param name="listofallboxesreferences">SortedList long, ConnectionRectangle, referenssi listaan, joka pitää kirjaa kaikista suorakulmioista jotka canvasille on piirretty </param>
        /// <param name="selectedcolor"> Brush, väri jolla viiva ja siihen liittyvä päätepiste halutaan piirtää </param>
        /// <returns> {int} Palauttaa 1, jos yhteyden luonti onnistui. Jos -1=epämääräinen virhe, -2=virhe objectindexerin kanssa ja -9=luokka initialisoitu väärin </returns>        
        public int AddSingleConnection(string kutsuja, Canvas origcanvas, long box1ownuid, long box2ownuid, SortedList<long, ConnectionRectangle> listofallboxesreferences, Brush selectedcolor)
        {
            string functionname = "->(CSH)AddSingleConnection";
            int retVal=-1;

            if (this.IsClassInitialized==true) {

                ConnectionRectangle tempconnectionrect;

                // Selvitetään omat ja vanhempien UID:it, jotka kirjataan sitten connectionille

                if (this.objindexerobj.objectlist.IndexOfKey(box1ownuid)>-1 && this.objindexerobj.objectlist.IndexOfKey(box2ownuid)>-1) {
                    long box1parentuid = this.objindexerobj.objectlist[box1ownuid].ParentUID;
                    long box2parentuid = this.objindexerobj.objectlist[box2ownuid].ParentUID;

                    long thisconnectionownUID = this.objindexerobj.AddObjectToIndexer(kutsuja + functionname, this.OwnUID, (int)ObjectIndexer.indexerObjectTypes.CONNECTION_LINE_110, -1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.ParentUID); // Parent uid on ConnectionsHandlerin UID ja Granparentuid on ActionCentreUI:n uid
                    if (thisconnectionownUID>=0) {

                        long connectionTextUID = this.objindexerobj.AddObjectToIndexer(kutsuja + functionname, thisconnectionownUID, (int)ObjectIndexer.indexerObjectTypes.CONNECTION_LINE_TEXT_111, -1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.OwnUID); // Granparent on ConnectionsHandlerin uid ja parent on Connection:in uid
                        if (connectionTextUID>=0) {

                            this.connections.Add(thisconnectionownUID, new Connection(kutsuja+functionname, this.prohmi, this.objindexerobj, thisconnectionownUID, this.OwnUID, this.ParentUID, box1ownuid, box1parentuid, box2ownuid, box2parentuid, this.createuicomps)); // Asetetaan itse tietorakenteen tiedot

                            int resp=this.objindexerobj.SetObjectToIndexerWithErrorReport(kutsuja+functionname,thisconnectionownUID,this.connections[thisconnectionownUID]);
                            if (resp<0) {
                                this.prohmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! Unsuccesful object set! OwnUID:"+this.OwnUID+" ConnectionUID:"+thisconnectionownUID+" Response:"+resp,-1270,4,4); 
                            }

                            // Asetetaan itse laatikoille myös tiedoksi connectionien UID luvut. Tämä on tärkeää, jotta laatikot tietävät, mitä connectioneja niihin on yhdistetty
                            if (listofallboxesreferences.IndexOfKey(box1ownuid)>-1) {
                                tempconnectionrect=listofallboxesreferences[box1ownuid];
                                tempconnectionrect.ConnectionUIDs.Add(thisconnectionownUID);
                            } else {
                                this.prohmi.sendError(kutsuja+functionname,"No such object in allboxeslist! Box1UID:"+box1ownuid,-731,4,4);
                            }

                            if (listofallboxesreferences.IndexOfKey(box2ownuid)>-1) {
                                tempconnectionrect=listofallboxesreferences[box2ownuid];
                                tempconnectionrect.ConnectionUIDs.Add(thisconnectionownUID);
                            } else {
                                this.prohmi.sendError(kutsuja+functionname,"No such object in allboxeslist! Box2UID:"+box2ownuid,-732,4,4);
                            }

                            if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                                retVal=this.connections[thisconnectionownUID].AddConnection(kutsuja+functionname,thisconnectionownUID,connectionTextUID,origcanvas,box1ownuid,box2ownuid,listofallboxesreferences,selectedcolor); // Asetetaan graafiset komponentit
                                retVal=retVal*10; // Antaa virheen -10, -20, -30 tai -40, koska on periytynyt aliluokista tai vaihtoehtoisesti arvon 1, jos kaikki onnistui
                            } else {
                                retVal=1;
                            }
                        } else {
                            retVal=-12;
                            this.prohmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+connectionTextUID+" Box1uid:"+box1ownuid+" Box2uid:"+box2ownuid,-1055,4,4);
                        }
                    } else {
                        retVal=-11;
                        this.prohmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+thisconnectionownUID+" Box1uid:"+box1ownuid+" Box2uid:"+box2ownuid,-1054,4,4);
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Objects wasn't added to ObjectIndexer! Box1uid:"+box1ownuid+" Box2uid:"+box2ownuid,-1053,4,4);
                    retVal=-2;
                }

            } else {
                this.prohmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassokay,-777,4,4);
                retVal=-9;
            }
            return retVal;               
        }

        /// <summary> Tekee saman kuin UpdateConnectionOnCanvas, eli päivittää yhden viivan sijainnin kahden yhdistämislaatikon väliltä sekä siihen liittyvät tekstit </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="updateuid"> long, connectionin uid, joka halutaan etsiä ja jolla halutaan yhteys päivittää </param>        
        /// <returns> {int} Jos 1=kohteen päivitys ruudulle onnistui, jos -1=määrittelemätön virhe, -2=ei kyseistä kohdetta connections listalla, -3=luokka initialisoitu väärin ja -4=yritys päivittää graafisia komponentteja, joita ei ole luotu </returns>
        public int UpdateConnectionOnCanvasWithUID(string kutsuja, long updateuid)
        {
            string functionname="->(CSH)UpdateConnectionWithUID";
            int retVal=-1;

            if (this.IsClassInitialized==true) {
                if (this.connections.IndexOfKey(updateuid)>-1) {
                    if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                        retVal=this.connections[updateuid].ReturnConnectionUIComponents.UpdateConnectionOnCanvas(kutsuja+functionname);
                        if (retVal<1) {
                            retVal=retVal-200; // Antaa epäonnistumisesta normaalia suuremman negatiivisen luvun, jota voidaan tulkita virheen loggauksessa
                        }
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Components not created! CreateUIComps state:"+this.createuicomps,-1046,4,4);
                        retVal=-4;
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"No such object UID in connections list! Seeked UID:"+updateuid,-737,4,4);
                    retVal=-2;
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassokay,-778,4,4);
                retVal=-3;
            }
            return retVal;
        }

    }

