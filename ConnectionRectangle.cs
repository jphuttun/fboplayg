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
    
    /// <summary>
    /// Tämä luokka pitää sisällään yhden listan ConnectionRectangleja. Tällä tavalla voidaan luokan sisäisellä propertyllä hoitaa IterateThrough kohdan ylläpito
    /// </summary>
    public class ConnectionRectangleList
    {
        /// <summary>
        /// Parent objektin äitiobjektin UID
        /// </summary>
        public long GranParentUID { get; set; } 

        /// <summary> Tämän listan oman vanhemman UID </summary>
        public long ParentUID { get; set; }

        /// <summary> Tämän listan objektin oma UID </summary>
        public long OwnUID { get; set; }

        /// <summary> Vasemmassa ja oikeassa laidassa olevat laatikot - indeksinä uniqrefnum (UID) </summary>
        private SortedList<long, ConnectionRectangle> boxlist;

        /// <summary> Vasemmassa ja oikeassa laidassa olevat laatikot - indeksinä uniqrefnum (UID). Palauttaa listan referenssin </summary>
        public SortedList<long, ConnectionRectangle> ReturnBoxList {
            get { return this.boxlist; }
        }

        /// <summary> Käyttöliittymäluokan referenssi </summary>
        private ProgramHMI prohmi;

        /// <summary>
        /// Referenssi luokkaan, joka hoitaa kohteiden ominaisuuksien printtauksen joko tallennusta tai ruutuun printtausta varten
        /// </summary>
        private ParamPrinter parameterprinter;

        /// <summary> Referenssi Connectionshandler luokkaan, joka pitää connection objektit järjestyksessä listassa </summary>
        private ConnectionsHandler connectionhandle;

        /// <summary>
        /// Tämä luokan instanssi pitää sisällään tarvittavat tiedot ParseJSON objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja
        /// </summary>
        private JsonParsingStruct jsonparsestructure;

        /// <summary> Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </summary>
        private ObjectIndexer objectindexer;        

        /// <summary>
        /// luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot ImportantProgramParams.createUIComponents enumeroinnista
        /// </summary>
        private int createuicomps=(int)ImportantProgramParams.createUIComponents.DO_NOT_CREATE_UI_COMPONENTS_0;

        /// <summary>
        /// Tämä muuttuja pitää kirjaa, missä elementissä ollaan menossa, jos iteroidaan koko komponentti läpi listan yksilö kerrallaan
        /// </summary>
        private int iteratethroughcounter=0;

        /// <summary>
        /// Tämä instanssi pitää kirjaa, onko instanssi initialisoitu oikein vai ei
        /// </summary> 
        private InitializationCounterClass classiniokay;

        /// <summary>
        /// Tämä muuttuja määrittelee, milloin instanssi katsotaan initialisoiduksi oikealla tavalla
        /// </summary>
        int classinimustexceed=0;                    

        /// <summary> Constructor - luo objektin instanssin, joka pitää sisällään listan pikkulaatikoita </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"> long, päälaatikon uniqrefnum jonka mukana nämä pikkulaatikot liikkuvat </param>
        /// <param name="granparentuid"> long, päälaatikon vanhemman uniqrefnum (UID)</param>
        /// <param name="jpstruct"> JsonParsingStruct, tämä referenssi pitää sisällään tarvittavat tiedot ParseJSON objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja </param>
        /// <param name="objindexer"> ObjectIndexer, Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </param>
        /// <param name="prohmi"> ProgramHMI, referenssi käyttöliittymäluokkaan </param>
        /// <param name="parampri"> ParamPrinter, referenssi parametri printteri luokan instanssiin </param>
        /// <param name="connh"> ConnectionsHandler, referenssi luokkaan, joka pitää connection objektit järjestyksessä listassa </param>
        /// <param name="creategraphcomp"> int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </param>
        /// <returns> {void} </returns>
        public ConnectionRectangleList(string kutsuja, long parentuid, long granparentuid, JsonParsingStruct jpstruct, ObjectIndexer objindexer, ProgramHMI prhmi, ParamPrinter parampri, ConnectionsHandler connh, int creategraphcomp=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
            string functionname="->(CRL)ConnectionRectangleList";
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.prohmi=prhmi;
            this.jsonparsestructure=jpstruct;
            this.objectindexer=objindexer;
            this.connectionhandle=connh;
            this.createuicomps=creategraphcomp;
            this.parameterprinter=parampri;

            this.classiniokay=new InitializationCounterClass(kutsuja+functionname,classinimustexceed);

            this.boxlist=new SortedList<long, ConnectionRectangle>();

            this.OwnUID=this.objectindexer.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.CONNECTION_RECTANGLE_LIST_104,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.GranParentUID); // Tässä -1 tarkoittaa, että objarraytype tietoa ei ole määritelty
            if (this.OwnUID<0) {
                this.prohmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+this.OwnUID+" ParentUID:"+parentuid,-1138,4,4);
            } else {
                this.classiniokay.AddClassOkayByNumber(kutsuja+functionname,1);
                int resp=this.objectindexer.SetObjectToIndexerWithErrorReport(kutsuja+functionname,this.OwnUID,this);
                if (resp>0) {
                    this.classiniokay.AddClassOkayByNumber(kutsuja+functionname,1);
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Unable to set object reference to objectindexer! UID:"+this.OwnUID,-1152,4,4);
                }
            }               
        }

        /// <summary>
        /// Kopioi kaikkien ConnectionRectangle-objektien BlockAtomValue-arvot BlockHandle-objektien BlockAtomValue-arvoihin.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <returns>{int} Palauttaa 2, jos boxlist lista tyhjä. Palauttaa 1, jos kaikki toimenpiteet onnistuivat. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos jokin toimenpide epäonnistui.</returns>
        public int CopyAllBlockAtomValuesToBlockHandles(string kutsuja)
        {
            string functionname = "->(CRL)CopyAllBlockAtomValuesToBlockHandles";
            ConnectionRectangle connRect=null;
            int cou=this.ReturnBoxList.Count();
            if (cou>0) {
                for (int i=0; i<cou; i++) {
                    connRect=this.boxlist.ElementAt(i).Value;
                    int result = connRect.CopyBlockAtomValueToBlockHandle(kutsuja + functionname);
                    if (result < 0) {
                        this.prohmi.sendError(kutsuja + functionname, "Failed to copy BlockAtomValue for ConnectionRectangle UID: " + connRect.OwnUID+" Response:"+result, -1325,4, 4);
                        return result; // Palautetaan ensimmäinen negatiivinen virhekoodi
                    }
                }
                return 1; // Kaikki toimenpiteet onnistuivat
            } else {
                return 2; // Ei yhtään kohdetta listassa, joten periaatteessa toiminto onnistui
            }
        }        

        /// <summary>
        /// Palauttaa ensimmäisen elementin ConnectionRectangleList:in boxlist listalta järjestyksessä
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns>{long} Palauttaa ensimmäisen kohteen UID numeron. Palauttaa -2, jos kohteita ei ollut listassa. Palauttaa -10, jos epämääräinen virhe. </returns>
        public long IterateThroughConnectionRectangleListsFirst(string kutsuja)
        {
            string functionname="->(CRL)IterateThroughConnectionRectangleListsFirst";
            long retVal=-10;
            
            if (this.classiniokay.IsClassInitialized==true) {

                if (this.boxlist.Count>0) {
                    retVal=this.boxlist.ElementAt(0).Key;
                    this.iteratethroughcounter=0;
                } else {
                    retVal=-2;
                }

            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1149,4,4);
            }            
            return retVal;
        }

        /// <summary>
        /// Tämä metodi antaa seuraavan kohteen UID:n boxlist listan kohteista. Jos käytetään UIDtoseek parametria, etsitään listasta UID ja palautetaan kyseisen listan elementistä seuraava kohde
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä boxlist listan listoilta</param>
        /// <returns>{long} Palauttaa boxlist listan kohteista seuraavan. Käytetään joko sisäistä counteria tai etsitään kohde ja palautetaan listassa seuraava. Jos suurempi tai yhtäsuuri kuin 0, niin kohteen UID, jos -1=mentiin listan indekseissä yli listan lopusta, -2=ei yhtään kohdetta listassa, -3=ei kyseistä UID tietoa boxlist listassa ja -10=epämääräinen virhe</returns>
        public long IterateThroughConnectionRectangleListsNext(string kutsuja, long UIDtoseek=-1)
        {
            long retVal=-10;
            int elemindex=-1;
            string functionname="->(CRL)IterateThroughConnectionRectangleListsNext";
            if (this.classiniokay.IsClassInitialized==true) {
                if (this.boxlist.Count>0) {
                    if (UIDtoseek>=0) { // Jos UIDtoseek on annettu, etsitään löytyykö kyseisellä UID:lla kohdetta
                        elemindex=this.boxlist.IndexOfKey(UIDtoseek);
                        if (elemindex<0) { // Kohdetta ei löydy listasta, joten palautetaan -3
                            retVal=-3;
                            return retVal;
                        } else {
                            this.iteratethroughcounter=elemindex+1; // Jos kohde löydetään, niin annetaan kohteen UID:ia seuraava kohde
                        }
                    } else {
                        this.iteratethroughcounter++; // Jos UIDtoseek tietoa ei ole annettu, niin mennään vain listassa seuraavaan kohteeseen.
                    }
                    if (this.iteratethroughcounter<this.boxlist.Count) {
                        retVal=this.boxlist.ElementAt(this.iteratethroughcounter).Key;
                    } else {
                        retVal=-1; // Jos ylitetään listalla olevien kohteiden lukumäärä, niin palautetaan tällöin -1
                    }
                } else {
                    retVal=-2; // Jos kohteita ei ollut yhtään listalla
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1150,4,4);
            }               
            return retVal;
        }

        /// <summary>
        /// Tämä metodi tuhoaa motherconnectionrectanglen alla olevat kaikki yhdistysrectanglet sekä niihin liittyvät connection kokoelman yhdistysviivat
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="origcanvas">Canvas, se canvas, josta connectionin graafiset komponentit poistetaan</param>
        /// <param name="collectionofallboxes">SortedList &lt; long, ConnectionRectangle &gt; , referenssi listaan, joka pitää kirjaa kaikista suorakulmioista jotka canvasille on piirretty </param>        
        /// <returns> {int} Palauttaa 1, jos sai poistettua Rectanglen komponentit sekä Connectionit koko RectangleList:stä, kuten myös niiden UI:n vastaavuudet. Jos palauttaa pienempi kuin 0, niin virhe! </returns>        
        public int DeleteConnectionRectangleList(string kutsuja, Canvas origcanvas, SortedList<long, ConnectionRectangle> collectionofallboxes)
        {
            string functionname="->(CRL)DeleteConnectionRectangle";
            int i=0;
            long keynum=-1;

            if (this.classiniokay.IsClassInitialized==true) {
                if (this.ReturnBoxList!=null) {
                    if (this.ReturnBoxList.Count>0) {
                        while (i<this.ReturnBoxList.Count) {

                            keynum=this.ReturnBoxList.ElementAt(i).Key;
                            
                            int respo=this.ReturnBoxList[keynum].RemoveRectangle(kutsuja+functionname,origcanvas,collectionofallboxes);
                            this.ReturnBoxList.Remove(keynum);
                            if (respo<1) {
                                this.prohmi.sendError(kutsuja+functionname,"Error during removing rectangle! Response:"+respo,-1343,4,4);
                                return respo;
                            }
                            i++;
                        }
                    }
                    return 1;
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Boxlist was null! UID:"+this.OwnUID,-1161,4,4);
                    return-91;
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1160,4,4);
                return -92;
            }
        }

        /// <summary>
        /// Tämä metodi palauttaa ConnectionRectangle objektin referenssin, jos löytää sen boxlist listalta UID:lla tai vaihtoehtoisesti null, mikäli ei löydä sitä
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä boxlist listan listoilta</param>
        /// <returns> {ConnectionRectangle}, tämä metodi palauttaa ConnectionRectangle objektin referenssin, jos löytää sen boxlist listalta UID:lla tai vaihtoehtoisesti null, mikäli ei löydä sitä</returns>
        public ConnectionRectangle ReturnConnectionRectangleByUID (string kutsuja, long UIDtoseek)
        {
            string functionname="->(CRL)ReturnConnectionRectanbleByUID";
            if (this.classiniokay.IsClassInitialized==true) {
                if (this.boxlist.IndexOfKey(UIDtoseek)>-1) {
                    return this.boxlist[UIDtoseek];
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Not such element in ListofConnectionRectangles! Seeking UID:"+UIDtoseek,-1137,4,4);
                    return null;
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1151,4,4);
                return null;
            }   
        } 

        /// <summary>
        /// Returns the first ConnectionRectangle object from the boxlist.
        /// </summary>
        /// <param name="kutsuja">The caller's path.</param>
        /// <returns>{ConnectionRectangle} Returns the first ConnectionRectangle object from the list or null if the list is empty or an error occurs.</returns>
        public ConnectionRectangle GetFirstConnectionRectangle(string kutsuja)
        {
            string functionname = "->(CRL)GetFirstConnectionRectangle";
            if (this.classiniokay.IsClassInitialized==true) {
                if (boxlist.Count > 0)
                {
                    long firstUID = boxlist.Keys[0];
                    this.iteratethroughcounter = 0;
                    return ReturnConnectionRectangleByUID(kutsuja, firstUID);
                }
                else
                {
                    this.prohmi.sendError(kutsuja + functionname, "The boxlist is empty.", -1153, 4, 4);
                    return null;
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1155,4,4);
                return null;
            }  
        }

        /// <summary>
        /// Iterates through the boxlist and returns the next ConnectionRectangle object.
        /// </summary>
        /// <param name="kutsuja">string, The caller's path.</param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä boxlist listan listoilta</param>
        /// <returns>{ConnectionRectangle} Returns the next ConnectionRectangle object from the list or null if there is no next object or an error occurs.</returns>
        public ConnectionRectangle GetNextConnectionRectangle(string kutsuja, long UIDtoseek=-1)
        {
            string functionname = "->(CRL)GetNextConnectionRectangle";

            if (this.classiniokay.IsClassInitialized==true) {
                long nextUID=this.IterateThroughConnectionRectangleListsNext(kutsuja+functionname,UIDtoseek);
                if (nextUID>=0) {
                    return ReturnConnectionRectangleByUID(kutsuja+functionname, nextUID);
                } else {
                    this.prohmi.sendError(kutsuja + functionname, "Error returning a next ConnectionRectangle object reference! Response:"+nextUID, -1154, 4, 4);
                    return null;
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1156,4,4);
                return null;
            }              
        }

    }

    /// <summary> Tämä luokka pitää sisällään pikkulaatikot, joita voi yhdistää toisiinsa isojen vetämällä viivoja isojen niiden välillä </summary>
    public class ConnectionRectangles
    {
        /// <summary>
        /// Parent objektin äitiobjektin UID
        /// </summary>
        public long GranParentUID { get; set; } 

        /// <summary> Päälaatikon uniqrefnum, jonka alla nämä pikkulaatikot sijaitsevat </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Tämän objektin latauksen yhteydessä löydetty ParentUID tallennettuna, koska muuten voi syntyä ristiriitoja yksilöllisten tunnusten kanssa.
        /// </summary>
        public long LoadedParentUID { get; set; }         

        /// <summary> Tämän pikkulaatikkoja sisällään pitävän luokan uniqrefnum </summary>
        public long ConnectionHolderUID { get; set; }

        /// <summary>
        /// Tämän objektin latauksen yhteydessä löydetty oma UID tallennettuna, koska muuten voi syntyä ristiriitoja yksilöllisten tunnusten kanssa.
        /// </summary>
        public long LoadedConnectionHolderUID { get; set; }

        /// <summary>
        /// Lista erivärisistä laatikkotyypeistä, joita on tallennettu ConnectionRectangleList listoihin
        /// </summary>
        public SortedList<int, ConnectionRectangleList> colorBoxes;       

        /// <summary> Päälaatikon vasemmassa laidassa olevat keltaiset laatikot - indeksinä uniqrefnum </summary>
        public ConnectionRectangleList yellowBoxes;

        /// <summary> Päälaatikon vasemmassa laidassa olevat vihreät laatikot - indeksinä uniqrefnum </summary>
        public ConnectionRectangleList greenBoxes;

        /// <summary> Päälaatikon oikeassa laidassa olevat punaiset laatikot - indeksinä uniqrefnum </summary>
        public ConnectionRectangleList redBoxes;

        /// <summary> Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </summary>
        private ObjectIndexer objectindexer;

        /// <summary> Käyttöliittymäluokan referenssi </summary>
        private ProgramHMI prohmi;

        /// <summary>
        /// Referenssi luokkaan, joka hoitaa kohteiden ominaisuuksien printtauksen joko tallennusta tai ruutuun printtausta varten
        /// </summary>
        private ParamPrinter parameterprinter;

        /// <summary> Referenssi Connectionshandler luokkaan, joka pitää connection objektit järjestyksessä listassa </summary>
        private ConnectionsHandler connectionhandle;

        /// <summary>
        /// Tämä luokan instanssi pitää sisällään tarvittavat tiedot ParseJSON objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja
        /// </summary>
        private JsonParsingStruct jsonparsestructure;

        /// <summary>
        /// luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista
        /// </summary>
        private int createuicomps=(int)ImportantProgramParams.createUIComponents.DO_NOT_CREATE_UI_COMPONENTS_0;

        /// <summary>
        /// Tämä instanssi pitää kirjaa, onko instanssi initialisoitu oikein vai ei
        /// </summary> 
        private InitializationCounterClass classiniokay;

        /// <summary>
        /// Tämä muuttuja määrittelee, milloin instanssi katsotaan initialisoiduksi oikealla tavalla
        /// </summary>
        private int classinimustexceed=0;

        /// <summary>
        /// Tämä muuttuja kertoo, missä listassa ollaan menossa
        /// </summary>
        private int boxtypecounter=-1;

        /// <summary>
        /// Palauttaa sen iterationthrough funktioiden sillä hetkellä aktiivisen iterationclass tiedon
        /// </summary>
        public int ReturnCurrentOngoingIterationClass {
            get { return this.boxtypecounter; }
        }

        /// <summary>
        /// Tämä muuttuja kertoo edellisen löytyneen ConnectionRectanglen UID:n
        /// </summary>
        private long connectionrectanglelistpreviouslyfounduid=-1;

        /// <summary>
        /// Tämä muuttuja kertoo edellisellä kerralla löytyneen ConnectionRectanglen UID:n Iterationthrough metodeja käyttäessä
        /// </summary>
        public long ReturnPreviouslyFoundConnectionRectangleUID {
            get { return this.connectionrectanglelistpreviouslyfounduid; }
        }

        /// <summary> Constructor - luo objektin instanssin, joka pitää sisällään ison sinisen laatikon sisälle tulevat pikkulaatikot </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"> long, päälaatikon uniqrefnum jonka mukana nämä pikkulaatikot liikkuvat </param>
        /// <param name="granparentuid"> long, päälaatikon vanhemman uniqrefnum (UID)</param>
        /// <param name="jpstruct"> JsonParsingStruct, tämä referenssi pitää sisällään tarvittavat tiedot ParseJSON objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja </param>
        /// <param name="objindexer"> ObjectIndexer, Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </param>
        /// <param name="prohmi"> ProgramHMI, referenssi käyttöliittymäluokkaan </param>
        /// <param name="parampri"> ParamPrinter, referenssi parametri printteri luokan instanssiin </param>
        /// <param name="connh"> ConnectionsHandler, referenssi luokkaan, joka pitää connection objektit järjestyksessä listassa </param>
        /// <param name="creategraphcomp"> int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </param>
        /// <returns> {void} </returns>
        public ConnectionRectangles(string kutsuja, long parentuid, long granparentuid, JsonParsingStruct jpstruct, ObjectIndexer objindexer, ProgramHMI prhmi, ParamPrinter parampri, ConnectionsHandler connh, int creategraphcomp=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1)
        {
            string functionname="->(CRS)Constructor";
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.prohmi=prhmi;
            this.jsonparsestructure=jpstruct;
            this.objectindexer=objindexer;
            this.connectionhandle=connh;
            this.createuicomps=creategraphcomp;

            //this.parameterprinter=parampri;
            this.parameterprinter = new ParamPrinter(kutsuja+functionname,this.prohmi,parampri.merkkijonokasittelija);            

            this.ConnectionHolderUID=this.objectindexer.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.CONNECTION_RECTANGLES_101,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.GranParentUID); // Tässä -1 tarkoittaa, että objarraytype tietoa ei ole määritelty
            this.classiniokay=new InitializationCounterClass(kutsuja+functionname,this.classinimustexceed);
            
            if (this.ConnectionHolderUID<0) {
                this.prohmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+this.ConnectionHolderUID+" ParentUID:"+parentuid,-1057,4,4);
            } else {
                    this.classiniokay.AddClassOkayByNumber(kutsuja+functionname,1);
                    this.colorBoxes = new SortedList<int, ConnectionRectangleList>();
                    this.yellowBoxes = new ConnectionRectangleList(kutsuja+functionname,this.ConnectionHolderUID,this.ParentUID,this.jsonparsestructure,this.objectindexer,this.prohmi,this.parameterprinter,this.connectionhandle,this.createuicomps);
                    this.greenBoxes = new ConnectionRectangleList(kutsuja+functionname,this.ConnectionHolderUID,this.ParentUID,this.jsonparsestructure,this.objectindexer,this.prohmi,this.parameterprinter,this.connectionhandle,this.createuicomps);
                    this.redBoxes = new ConnectionRectangleList(kutsuja+functionname,this.ConnectionHolderUID,this.ParentUID,this.jsonparsestructure,this.objectindexer,this.prohmi,this.parameterprinter,this.connectionhandle,this.createuicomps);
                    this.colorBoxes.Add((int)connectionBoxType.YELLOW_BOX_COMPARE_VALUE_1,this.yellowBoxes);
                    this.colorBoxes.Add((int)connectionBoxType.GREEN_BOX_CHECK_VALUE_2,this.greenBoxes);
                    this.colorBoxes.Add((int)connectionBoxType.RED_BOX_RESULT_VALUE_3,this.redBoxes);
                if (this.objectindexer.objectlist.IndexOfKey(this.ConnectionHolderUID)>-1) {
                    int resp=this.objectindexer.SetObjectToIndexerWithErrorReport(kutsuja+functionname,this.ConnectionHolderUID,this);
                    if (resp<0) {
                        this.prohmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! Unsuccesful object set! UID:"+this.ConnectionHolderUID+" Response:"+resp,-1123,4,4); 
                    } else {
                        this.classiniokay.AddClassOkayByNumber(kutsuja+functionname,1);
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Fatal error! Problem to set object to objectindexer! No entry found! UID:"+this.ConnectionHolderUID,-1124,4,4);
                }
            }          
        }

        /// <summary>
        /// Kopioi kaikkien iterationclass arvoa vastaavien ConnectionRectangleList-objektien BlockAtomValue-arvot BlockHandle-objektien BlockAtomValue-arvoihin.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="iterationclass">Iterationclass arvo, joka on sama kuin ConnectionRectangles.connectionBoxType enumeraation arvo. Jos tämä arvo on 0, kopioidaan kaikki kohteet.</param>
        /// <returns>{int} Palauttaa 1, jos kaikki toimenpiteet onnistuivat. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos jokin toimenpide epäonnistui.</returns>
        public int CopyAllBlockAtomValuesByIterationClass(string kutsuja, int iterationclass)
        {
            string functionname = "->(CRS)CopyAllBlockAtomValuesByIterationClass";
            
            if (iterationclass == (int)ConnectionRectangles.connectionBoxType.ALL_BOXES_0) {
                foreach (var entry in this.colorBoxes) {
                    int result = entry.Value.CopyAllBlockAtomValuesToBlockHandles(kutsuja + functionname);
                    if (result < 0) {
                        this.prohmi.sendError(kutsuja + functionname, "Failed to copy BlockAtomValue for ConnectionRectangleList with iteration class: " + entry.Key + " Response:" + result, -1326, 4, 4);
                        return result; // Palautetaan ensimmäinen negatiivinen virhekoodi
                    }
                }
                return 1; // Kaikki toimenpiteet onnistuivat
            } else {
                if (this.colorBoxes.ContainsKey(iterationclass)) {
                    int result = this.colorBoxes[iterationclass].CopyAllBlockAtomValuesToBlockHandles(kutsuja + functionname);
                    if (result < 0) {
                        this.prohmi.sendError(kutsuja + functionname, "Failed to copy BlockAtomValue for ConnectionRectangleList with iteration class: " + iterationclass + " Response:" + result, -1327, 4, 4);
                        return result; // Palautetaan ensimmäinen negatiivinen virhekoodi
                    }
                    return 1; // Kaikki toimenpiteet onnistuivat
                } else {
                    this.prohmi.sendError(kutsuja + functionname, "Invalid iteration class: " + iterationclass, -1328, 4, 4);
                    return -1; // Virheellinen iterationclass
                }
            }
        }


        /// <summary> Tämä metodi päivittää kaikkien pikkulaatikoiden sijainnit, kun isoa laatikkoa siirretään toiseen kohtaan vetämällä </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>        
        /// <param name="newTop"> double, ison "äitilaatikon" uusi korkeusasema </param>
        /// <param name="newLeft"> double, ison "äitilaatikon" uusi leveysasema </param>
        /// <param name="mainwidth"> double, päälaatikon leveys </param>
        /// <param name="smallboxwidth"> double, pikkulaatikon leveys </param>
        /// <param name="heightLeft"> double, vasemman laidan pikkulaatikkojen korkeus </param>
        /// <param name="heightRight"> double, oikean laidan pikkulaatikkojen korkeus </param>
        /// <param name="letterOffsetLeft"> double, laatikon teksitikirjaimen offset yhdyslaatikolle sen vasemmasta reunasta </param>
        /// <param name="letterOffsetTop"> double, laatikon teksitikirjaimen offset yhdyslaatikolle sen yläreunasta </param>
        /// <returns> {void} </returns>
        public void UpdatePositionsDuringDrag(string kutsuja, double newTop, double newLeft, double mainwidth, double smallboxwidth, double heightLeft, double heightRight, double letterOffsetLeft, double letterOffsetTop)
        {
            string functionname="->(CRS)UpdatePositionsDuringDrag";
            if (this.classiniokay.IsClassInitialized==true) {
                int i=0;
                // Päivitetään vasemman reunan keltaiset ja vihreät laatikot
                if (this.yellowBoxes.ReturnBoxList.Count>0) {
                    foreach(var tempconrect in yellowBoxes.ReturnBoxList) {
                        tempconrect.Value.UpdatePositionsDuringDrag(kutsuja+functionname,newTop+(i*heightLeft),newLeft, letterOffsetLeft, letterOffsetTop);
                        i++;
                    }
                }
                if (this.greenBoxes.ReturnBoxList.Count>0) {
                    foreach(var tempconrect in greenBoxes.ReturnBoxList) {
                        tempconrect.Value.UpdatePositionsDuringDrag(kutsuja+functionname,newTop+(i*heightLeft),newLeft, letterOffsetLeft, letterOffsetTop);
                        i++;
                    }
                }

                // Päivitetään oikean reunan punaiset laatikot
                i=0;
                if (this.redBoxes.ReturnBoxList.Count>0) {
                    foreach(var tempconrect in redBoxes.ReturnBoxList) {
                        tempconrect.Value.UpdatePositionsDuringDrag(kutsuja+functionname,newTop+(i*heightRight),newLeft+mainwidth-smallboxwidth, letterOffsetLeft, letterOffsetTop);
                        i++;
                    }
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1141,4,4);
            }                       
        }

        /// <summary>
        /// Tämä metodi palauttaa ConnectionRectangle objektin referenssin, vaikka se ei kuuluisi tämän listan kohteisiin, jos kohde on vain rekisteröity objectindexeriin
        /// </summary>
        /// <param name="kutsuja">string, The caller's path.</param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä boxlist listan listoilta</param>
        /// <returns>{ConnectionRectangle} Palauttaa ConnectionRectangle objektin referenssin ObjectIndexeristä, jos sellainen löytyy UIDtoseek muuttujalla ja null, jos tuli virhe joko etsinnässä tai castauksessa. </returns>
        public ConnectionRectangle GetConnectionRectangleThroughObjectIndexer(string kutsuja, long UIDtoseek)
        {
            string functionname="->(CRL)GetConnectionRectangleThroughObjectIndexer";

            if (this.classiniokay.IsClassInitialized==true) {
                var obj=this.objectindexer.GetObjectFromIndexer(kutsuja+functionname,UIDtoseek);
                if (obj!=null) {
                    ConnectionRectangle connrect;
                    connrect=obj as ConnectionRectangle;
                    if (connrect!=null) {
                        return connrect;
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Object wasn't ConnectionRectangle! UID:"+UIDtoseek,-1166,4,4);
                        return null;                    
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Error with seeking object reference from ObjectIndexer! UID:"+UIDtoseek,-1165,4,4);
                    return null;
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1167,4,4);
                return null;
            }              
        }

        /// <summary>
        /// Tämä metodi käy läpi kaikki kohteet ConnectionReferenceListin ConnectionRectangle objekteista ja palauttaa ensimmäisen vastaan tulevan Connection objektin
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="iterationclass"> int, enumeraatio joka kertoo minkä listan kohteita käydään läpi ConnectionRectangleList objekteista. Ks. ConnectionRectangles.connectionBoxType </param>
        /// <returns>{Connection} palauttaa ensimmäisen vastaan tulevan Connection objektin ja virheen tapauksessa palauttaa null sekä antaa virheilmoituksen. Jos kohteet loppuivat tai niitä ei ollut, niin palauttaa myös null. </returns>
        public Connection IterateThrougConnectionsFirstReturnObject(string kutsuja, int iterationclass=(int)connectionBoxType.ALL_BOXES_0)
        {
            string functionname="->(CRS)IterateThroughConnectionsFirstReturnObject";
            long retVal;

            if (this.classiniokay.IsClassInitialized==true) {
                retVal=this.IterateThroughConnectionsFirst(kutsuja,iterationclass);
                if (retVal>=0) {
                    Connection conn=this.connectionhandle.GetConnectionByUID(kutsuja+functionname,retVal);
                    if (conn==null) {
                        this.prohmi.sendError(kutsuja+functionname,"Error during fetching Connection object by UID! UID:"+retVal,-1176,4,4);
                    }
                    return conn;
                } else if (retVal>=-2) { // Ei ollut enää kohteita jäljellä
                    return null;
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Error returned during fetching first Connection object! Response:"+retVal,-1175,4,4);
                    return null;
                }
            } else {                
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1174,4,4);
                return null;
            }            
        }

        /// <summary>
        /// Tämä metodi palauttaa ConnectionRectangleList listojen ConnectionRectangleista ensimmäisen eteen tulevan Connectionin UID:n
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="iterationclass"> int, enumeraatio joka kertoo minkä listan kohteita käydään läpi ConnectionRectangleList objekteista. Ks. ConnectionRectangles.connectionBoxType </param>
        /// <returns>{long} palauttaa ConnectionRectangleList listojen ConnectionRectangleista ensimmäisen eteen tulevan Connectionin UID:n. Jos -20=epämääräinen virhe, -21=virhe ObjectIndexerin objektin castauksessa tms., -2=ei kohteita viimeisimmällä listalla, -1=mentiin indekseissä yli viimeisen kohteen</returns>
        public long IterateThroughConnectionsFirst(string kutsuja, int iterationclass=(int)connectionBoxType.ALL_BOXES_0)
        {
            string functionname="->(CRS)IterateThroughConnectionsFirst";
            long firstRectUID;
            long nextRectUID;
            long retVal=-20;
            int iloop=-1;
            ConnectionRectangle connrect;
            if (this.classiniokay.IsClassInitialized==true) {
                firstRectUID=this.IterateThroughConnectionRectangleListFirst(kutsuja+functionname,iterationclass);
                if (firstRectUID>=0) {
                    connrect=this.GetConnectionRectangleThroughObjectIndexer(kutsuja+functionname,firstRectUID); // Otetaan connectionrectanglen referenssi, josta löytyy connectionit jotka on syytä käydä läpi
                    if (connrect!=null) {
                        retVal=connrect.IterateThroughConnectionsFirst(kutsuja+functionname);
                        if (retVal<0) { // Jos ensimmäisessä ConnectionRectanglessa ei ollut yhtään connectionia, niin etsitään niin pitkään että on käyty kaikki läpi tai löytyy jokin ConnectionRectangle, josta löytyy Connection
                            iloop=0;
                            while (iloop<100) // Jos jää jotenkin jumiin, niin iloop keskeyttää 100 iteraation jälkeen
                            {
                                nextRectUID=this.IterateThroughConnectionRectangleListNext(kutsuja+functionname,iterationclass);
                                if (nextRectUID>=0) {
                                    connrect=this.GetConnectionRectangleThroughObjectIndexer(kutsuja+functionname,nextRectUID); // Otetaan connectionrectanglen referenssi, josta löytyy connectionit jotka on syytä käydä läpi
                                    if (connrect!=null) {
                                        retVal=connrect.IterateThroughConnectionsFirst(kutsuja+functionname);
                                        if (retVal>=0) {
                                            return retVal; // Jos löytyi Connectionin UID, niin palautetaan se
                                        } else if (retVal<-2) {
                                            this.prohmi.sendError(kutsuja+functionname,"Error during seeking first Connection element! Response:"+nextRectUID,-1171,4,4);
                                            return retVal;
                                        }
                                        // Jatketaan vain, jos vastauksena tuli -2 tai -1
                                    } else {
                                        retVal=-21;
                                        this.prohmi.sendError(kutsuja+functionname,"Found UID wasn't in the ObjectIndexer! UID:"+nextRectUID,-1170,4,4);
                                        return retVal;
                                    }
                                } else if (nextRectUID>=-2) { // Ei enempää ConnectionRectangleja, joita olisi löytynyt
                                    return nextRectUID;
                                } else { // Virhe ConnectionRectanglen etsimisessä
                                    retVal=nextRectUID;
                                    this.prohmi.sendError(kutsuja+functionname,"Error during seeking first Connection element! Response:"+nextRectUID,-1169,4,4);
                                    return retVal;
                                }
                                iloop++;
                            }
                        }
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Found UID wasn't in the ObjectIndexer! UID:"+firstRectUID,-1168,4,4);
                    }
                } else if (firstRectUID>=-2) {
                    return firstRectUID; // Jos jonossa ei ole yhtään kohdetta odottamassa, niin palautetaan miinusmerkkinen arvo
                } else {
                    retVal=firstRectUID;
                    this.prohmi.sendError(kutsuja+functionname,"Error during seeking first Connection element! Response:"+firstRectUID,-1164,4,4);
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1162,4,4);
            }
            return retVal; 
        }

        /// <summary>
        /// Tämä metodi palauttaa ConnectionRectangleList listojen ConnectionRectangleista seuraavan eteen tulevan Connectionin UID:n
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="iterationclass"> int, enumeraatio joka kertoo minkä listan kohteita käydään läpi ConnectionRectangleList objekteista. Ks. ConnectionRectangles.connectionBoxType </param>
        /// <returns>{long} palauttaa ConnectionRectangleList listojen ConnectionRectangleista seuraavan eteen tulevan Connectionin UID:n. Jos -30=epämääräinen virhe, -31=luokka initialisoitu väärin, -32=aiempi UID oli väärin (ehkä First käsky kutsumatta ensin), -33=virhe ObjectIndexerin objektin castauksessa tms., -2=ei kohteita viimeisimmällä listalla, -1=mentiin indekseissä yli viimeisen kohteen</returns>
        public long IterateThroughConnectionsNext(string kutsuja, int iterationclass=(int)connectionBoxType.ALL_BOXES_0)
        {
            string functionname="->(CRS)IterateThroughConnectionsNext";
            long retVal=-30;
            ConnectionRectangle connrect;
            long seekedUID;

            if (this.classiniokay.IsClassInitialized==true) {
                seekedUID=this.ReturnPreviouslyFoundConnectionRectangleUID;
                if (seekedUID>=0) { // Jos edellinen ConnectionRectanglen UID oli validi ..
                    int iloop=0;
                    while (iloop<100) { // 100 iteraatiokertaa riittää, jos jää jotenkin jumiin
                        connrect=this.GetConnectionRectangleThroughObjectIndexer(kutsuja+functionname,seekedUID); // .. niin haetaan kyseisen ConnectionRectanglen objekti ..
                        if (connrect!=null) {
                            retVal=connrect.IterateThroughConnectionsNext(kutsuja+functionname); // .. ja objektista seuraava connection, joka on siellä listoilla odottamassa ..
                            if (retVal>=0) {
                                return retVal; // .. ja palautetaan se!
                            } else if (retVal>=-2 && retVal<0) { // .. paitsi, jos listassa ei ollut enempää Connection:ja, vaan pitää siirtyä seuraavaan ConnectionRectangleen
                                long fetchconnrectUID=this.IterateThroughConnectionRectangleListNext(kutsuja+functionname,iterationclass);
                                if (fetchconnrectUID>=0) {
                                    seekedUID=fetchconnrectUID; 
                                } else if (fetchconnrectUID>=-2 && fetchconnrectUID<0) {
                                    return fetchconnrectUID; // Palautetaan tieto, että enää ei ollut yhtään listoilla ConnectionRectanglea, josta olisi voitu kysellä Connectioneja
                                } else {
                                    this.prohmi.sendError(kutsuja+functionname,"Error during seeking next ConnectionRectangle element! Response:"+fetchconnrectUID,-1180,4,4); // Virhe etsittäessä seuraavaa ConnectionRectanglea
                                    return fetchconnrectUID;   
                                }
                            } else { // .. tai, jos tuli virhe kyseltäessä seuraavaa Connection kohdetta
                                this.prohmi.sendError(kutsuja+functionname,"Error to fetch Connection UID! Response:"+retVal,-1182,4,4);
                                return retVal;
                            }
                        } else {
                            retVal=-33;                        
                            this.prohmi.sendError(kutsuja+functionname,"Error to fetch ConnectionRectangle from ObjectIndexer! Response:"+retVal,-1181,4,4);
                            return retVal;
                        }
                        iloop++;
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Previous connection UID was invalid! Did you call First method first? Response:"+seekedUID,-1179,4,4);
                    retVal=-32;
                }

            } else {
                retVal=-31;
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1163,4,4);
            }   
            return retVal;
        }      

        /// <summary>
        /// Tämä metodi käy tarvittaessa läpi kaikki tämän ConnectionRectangles objektin ConnectionRectangleList objektien boxlist:it ja palauttaa ensimmäisen listalla eteen tulevan kohteen UID:n
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="iterationclass"> int, enumeraatio joka kertoo minkä listan kohteita käydään läpi ConnectionRectangleList objekteista. Ks. ConnectionRectangles.connectionBoxType </param>
        /// <returns>{long} palauttaa ensimmäisen listalla eteen tulevan kohteen UID:n. Jos -10=epämääräinen virhe alifunktiossa, -11=ei etsittyä listatyyppiä, -12=epämääräinen virhe pääfunktiossa, -2=ei objekteja läpikäydyissä kohteissa</returns>
        public long IterateThroughConnectionRectangleListFirst(string kutsuja, int iterationclass=(int)connectionBoxType.ALL_BOXES_0)
        {
            string functionname="->(CRS)IterateThroghConnectionsFirst";
            long retVal=-12;

            if (this.classiniokay.IsClassInitialized==true) {
                int i=1;
                int j=1;
                int k=(int)connectionBoxType.MIN_VALUE_INDEX;
                if (iterationclass==(int)connectionBoxType.ALL_BOXES_0) {
                    j=(int)connectionBoxType.MAX_VALUE_INDEX; // Erityyppisten listojen määrä komponentissa, tässä yellow, green ja red
                } else { // Jos iteraatiotapa on käydä läpi vain iterationclass listan kohteet
                    k=iterationclass;
                    j=k;
                } 

                for (i=k; i<=j; i++) { // Käydään kaikki 3 listaa läpi tarvittaessa

                    this.boxtypecounter=i; // Otetaan talteen, mikä iterationclass on kyseessä

                    if (i>=(int)connectionBoxType.MIN_VALUE_INDEX && i<=(int)connectionBoxType.MAX_VALUE_INDEX) {
                        retVal=this.colorBoxes[i].IterateThroughConnectionRectangleListsFirst(kutsuja+functionname);
                        if (retVal>=0) {
                            this.connectionrectanglelistpreviouslyfounduid=retVal;
                            return retVal;
                        }
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Not any known connectionrectangle type! Type number:"+i+" Responserightnow:"+retVal+" Iterationclass:"+iterationclass,-1157,4,4);
                        this.connectionrectanglelistpreviouslyfounduid=-11;
                        retVal=-11;                        
                    }

                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1142,4,4);
            }

            this.connectionrectanglelistpreviouslyfounduid=-2;
            return retVal;   
        }

        /// <summary>
        /// Tämä metodi käy tarvittaessa läpi kaikki tämän ConnectionRectangles objektin ConnectionRectangleList objektien boxlist:it ja palauttaa seuraavan listalla eteen tulevan kohteen UID:n. Pitää itsenäisisesti kirjaa, missä listassa ollaan menossa ja mikä oli edellisen löydety ConnectionRectanglen UID. 
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="iterationclass"> int, enumeraatio joka kertoo minkä listan kohteita käydään läpi ConnectionRectangleList objekteista. Ks. ConnectionRectangles.connectionBoxType </param>
        /// <returns>{long} palauttaa seuraavan listalla eteen tulevan kohteen UID:n. Jos -10=epämääräinen virhe alifunktiossa, -11=ei etsittyä listatyyppiä, -12=epämääräinen virhe pääfunktiossa, -2=ei objekteja läpikäydyissä kohteissa, -1=saavutettiin indeksoinnin viimeinen kohde</returns>
        public long IterateThroughConnectionRectangleListNext(string kutsuja, int iterationclass=(int)connectionBoxType.ALL_BOXES_0)
        {
            string functionname="->(CRS)IterateThroughConnectionRectangleListNext";
            long retVal=-12;
            if (this.classiniokay.IsClassInitialized==true) {
                int i=1;
                int j=1;
                int k=(int)connectionBoxType.MIN_VALUE_INDEX;
                if (iterationclass==(int)connectionBoxType.ALL_BOXES_0) {
                    j=(int)connectionBoxType.MAX_VALUE_INDEX; // Erityyppisten listojen määrä komponentissa, tässä yellow, green ja red
                } else { // Jos iteraatiotapa on käydä läpi vain iterationclass listan kohteet
                    k=iterationclass;
                    j=k;
                } 

                k=this.boxtypecounter; // Edellisellä kierroksella enumeraatiokohteen int arvo, josta tällä kertaa jatketaan

                for (i=k; i<=j; i++) { // Käydään kaikki 3 listaa läpi tarvittaessa

                    this.boxtypecounter=i; // Otetaan talteen, mikä iterationclass on kyseessä

                    if (i>=(int)connectionBoxType.MIN_VALUE_INDEX && i<=(int)connectionBoxType.MAX_VALUE_INDEX) {
                        if (this.connectionrectanglelistpreviouslyfounduid<0) {
                            retVal=this.colorBoxes[i].IterateThroughConnectionRectangleListsFirst(kutsuja+functionname);
                        } else {
                            retVal=this.colorBoxes[i].IterateThroughConnectionRectangleListsNext(kutsuja+functionname,this.connectionrectanglelistpreviouslyfounduid);
                        }
                        
                        if (retVal>=-2 && retVal<0) {
                            this.connectionrectanglelistpreviouslyfounduid=retVal;
                        } else if (retVal>=0) {
                            this.connectionrectanglelistpreviouslyfounduid=retVal;
                            return retVal;
                        } else {
                            this.prohmi.sendError(kutsuja+functionname,"Error during seeking next connectionrectangle! Type number:"+i+" Responserightnow:"+retVal+" Iterationclass:"+iterationclass,-1158,4,4);
                            return retVal;
                        }
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Not any known connectionrectangle type! Type number:"+i+" Responserightnow:"+retVal+" Iterationclass:"+iterationclass,-1159,4,4);
                        this.connectionrectanglelistpreviouslyfounduid=-11;
                        retVal=-11;                      
                    }
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1143,4,4);
            }
            this.connectionrectanglelistpreviouslyfounduid=-2;
            return retVal;               
        }

        /// <summary>
        /// Tämä metodi tuhoaa motherconnectionrectanglen alla olevat kaikki yhdistysrectanglet sekä niihin liittyvät connection kokoelman yhdistysviivat tuhoamalla connectionrectanglelist objektit
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="origcanvas">Canvas, se canvas, josta connectionin graafiset komponentit poistetaan</param>
        /// <param name="collectionofallboxes">SortedList &lt; long, ConnectionRectangle &gt; , referenssi listaan, joka pitää kirjaa kaikista suorakulmioista jotka canvasille on piirretty </param>        
        /// <returns> {int} Palauttaa 1, jos sai poistettua RectangleList komponentit sekä Connectionit koko RectangleList:stä, kuten myös niiden UI:n vastaavuudet. Jos palauttaa &lt; 0, niin virhe! </returns>        
        public int DeleteConnectionRectangles(string kutsuja, Canvas origcanvas, SortedList<long, ConnectionRectangle> collectionofallboxes)
        {
            string functionname="->(CRS)DeleteConnectionRectangles";
            int i=0;

            if (this.classiniokay.IsClassInitialized==true) {

                for (i=(int)connectionBoxType.MIN_VALUE_INDEX; i<=(int)connectionBoxType.MAX_VALUE_INDEX; i++)
                {
                    int respo=this.colorBoxes[i].DeleteConnectionRectangleList(kutsuja+functionname,origcanvas,collectionofallboxes);
                    if (respo<1) {
                        this.prohmi.sendError(kutsuja+functionname,"Error with removing connectionrectanglelists! Index:"+i+" Response:"+respo,-1344,4,4);
                        return respo;
                    }
                }
                return 1;
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1144,4,4);
                return -81;
            }   
        }



        /// <summary> Tämä enum lista pitää sisällään minkä värisiä pikkulaatikoita luodaan ja mitä värit merkitsevät </summary>
        public enum connectionBoxType {
            /// <summary>
            /// Tieto, mikä on annetuista laatikoista pienin indeksinumero, kun all_boxes vaihtoehtoa ei lasketa
            /// </summary>
            MIN_VALUE_INDEX=1,
            /// <summary>
            /// Tieto, mikä on annetuista laatikoista suurin indeksinumero
            /// </summary>
            MAX_VALUE_INDEX=3,
            /// <summary>
            /// Tulopuolen maksimi arvo listan indekseissä
            /// </summary> 
            MAX_VALUE_INDEX_FOR_INCOMING_HANDLES=2,            
            /// <summary>
            /// Kaikkien tyyppiset laatikot - tämä on mahdollista valita vain IterateThrough käskyjä varten
            /// </summary>
            ALL_BOXES_0=0,
            /// <summary> Arvo, jota vasten lukua tarkistetaan </summary>
            YELLOW_BOX_COMPARE_VALUE_1=1,
            /// <summary> Arvo, jota testataan ehtolauseella </summary>
            GREEN_BOX_CHECK_VALUE_2=2,
            /// <summary> Tulosarvo, jonka esim. vertailu tai jokin muu blokki tuottaa </summary>
            RED_BOX_RESULT_VALUE_3=3
        }

        /// <summary> Tämä metodi luo halutun tyyppisen pikkulaatikon ja palauttaa ConnectionRectangle luokan referenssin </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"> long, päälaatikon uniqrefnum jonka mukana nämä pikkulaatikot liikkuvat </param>
        /// <param name="granparentuid"> long, päälaatikon uniqrefnumin äitiobjektin uid </param>
        /// <param name="boxtype"> int, 1=yellowBox, 2=greenBox, 3=redBox - lisää enum ConnectionRectangles.connectionBoxType </param>
        /// <param name="boxletter"> string, kirjain, joka tulostetaan rectanglen yläosaan </param>
        /// <returns> {ConnectionRectangle} palauttaa luodun ConnectionRectangle instanssin referenssin. Jos ei pysty palauttamaan objektia, niin palauttaa siinä tapauksessa null </returns>
        public ConnectionRectangle AddBox(string kutsuja, long parentuid, long granparentuid, int boxtype, string boxletter)
        {
            string functionname="->(CRS)AddBox";
            long newuniqnum=-1;
            ConnectionRectangle connrect;

            if (this.classiniokay.IsClassInitialized==true) {
                if (boxtype>=(int)connectionBoxType.MIN_VALUE_INDEX && boxtype<=(int)connectionBoxType.MAX_VALUE_INDEX) { // colorBoxes

                    connrect=new ConnectionRectangle(kutsuja+functionname,parentuid,granparentuid,this.objectindexer,this.prohmi,this.parameterprinter,this.connectionhandle, boxletter,out newuniqnum);                    
                    if (newuniqnum>=0) {
                        if (connrect!=null) {
                            this.colorBoxes[boxtype].ReturnBoxList.Add(newuniqnum, connrect);
                        } else {
                            this.prohmi.sendError(kutsuja+functionname,"Created ConnectionRectangle was null! UID:"+newuniqnum+" Boxtype:"+boxtype+" Response:"+newuniqnum+" ParentUid:"+parentuid,-1305,4,4);
                        }
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Boxtype:"+boxtype+" Response:"+newuniqnum+" ParentUid:"+parentuid,-1058,4,4);
                        connrect=null;
                    }
                    //connrect=this.colorBoxes[boxtype].ReturnBoxList[newuniqnum];

                /*
                } else if (boxtype==(int)connectionBoxType.GREEN_BOX_CHECK_VALUE_2) { // greenBox
                    newuniqnum=this.objectindexer.AddObjectToIndexer(kutsuja+functionname,parentuid,(int)ObjectIndexer.indexerObjectTypes.MOTHER_COMPONENT_RECTANGLE_102,-1);
                    if (newuniqnum>=0) {
                        this.greenBoxes.Add(newuniqnum, new ConnectionRectangle(kutsuja+functionname,this.ParentUID,this.objectindexer,this.prohmi,this.parameterprinter,this.connectionhandle, boxletter));
                        connrect=this.greenBoxes[newuniqnum];
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+newuniqnum+" ParentUid:"+parentuid,-1059,4,4);
                        connrect=null;
                    }                
                } else if (boxtype==(int)connectionBoxType.RED_BOX_RESULT_VALUE_3) { // redBox
                    newuniqnum=this.objectindexer.AddObjectToIndexer(kutsuja+functionname,parentuid,(int)ObjectIndexer.indexerObjectTypes.MOTHER_COMPONENT_RECTANGLE_102,-1);
                    if (newuniqnum>=0) {
                        this.redBoxes.Add(newuniqnum, new ConnectionRectangle(kutsuja+functionname,this.ParentUID,this.objectindexer,this.prohmi,this.parameterprinter,this.connectionhandle, boxletter));
                        connrect=this.redBoxes[newuniqnum];
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+newuniqnum+" ParentUid:"+parentuid,-1060,4,4);
                        connrect=null;
                    }
                */
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Invalid ConnectionRectangle type! Type:"+boxtype+" ParentUid:"+parentuid,-728,4,4);
                    return null;
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1145,4,4);
                connrect=null;
            }   
            return connrect;
        }

        /// <summary>
        /// Tämä metodi lisää ConnectionRectangles objektiin sen aliobjektit sen perusteella, mikä lista sille on annettu asetettavaksi
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="listtobeset">SortedList long, ConnectionRectangle - yellowBoxes, greenBoxes tai redBoxes lista johon sijoitetaan kohteiden arvoja</param>
        /// <param name="jsonobject">string, JSONobjekti yellowBoxes, greenBoxes tai redBoxes kohteen alta</param>
        /// <param name="isloaded"> int, jos 0, niin ei olle tietojen latauksesta kyse ja jos 1, niin UID tiedot menevät erillisiin Loaded muuttujiin varsinaisten muuttujien sijasta </param>
        /// <returns> {void} </returns>
        public void SetConnectionRectanglesColorLists(string kutsuja, ConnectionRectangleList listtobeset, string jsonobject, int isloaded)
        {
            string functionname="->(CRS)SetConnectionRectanglesColorLists";
            SortedDictionary<string, string> smallrectconndict;
            long rememparseuid=-1;
            long rememparseuidtwo=-1;

            if (this.classiniokay.IsClassInitialized==true) {

                if (this.jsonparsestructure!=null) {
                    if (jsonobject!=null && jsonobject!="" && jsonobject!="{}") {                       
                            rememparseuid=this.jsonparsestructure.Parserunninguid;
                            smallrectconndict=this.jsonparsestructure.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,this.jsonparsestructure.Parserunninguid.ToString(),jsonobject,-1); // Tässä -1 tarkoittaa, ettei tehdä debug printtausta
                            this.jsonparsestructure.Parserunninguid++;

                            if (smallrectconndict!=null) {
                                int amo=smallrectconndict.Count();
                                if (amo>0) {
                                    for (int i=0; i<amo; i++) {
                                        if (listtobeset.ReturnBoxList.Count()>i) {
                                            rememparseuidtwo=this.jsonparsestructure.Parserunninguid;
                                            SortedDictionary<string, string> conndict=this.jsonparsestructure.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,this.jsonparsestructure.Parserunninguid.ToString(),smallrectconndict.ElementAt(i).Value,-1); // Tässä -1 tarkoittaa, ettei tehdä debug printtausta
                                            this.jsonparsestructure.Parserunninguid++;
                                            if (conndict!=null && conndict.Count()>0) {                                        
                                                // TODO: Koska kohteet on luotu jo etukäteen, niin tässä kohdin ajatellaan, että ne on luotu samassa järjestyksessä, joka ei välttämättä pidä paikkaansa. Tee sellainen järjestelmä, jossa kohteet täsmätään toisiinsa
                                                this.parameterprinter.SetConnectionRectangleObjectToPrint(listtobeset.ReturnBoxList.ElementAt(i).Value);
                                                this.parameterprinter.SetConnectionRectangleParamValues(kutsuja+functionname,conndict,isloaded);
                                            } else {
                                                this.prohmi.sendError(kutsuja+functionname,"ConnectionRectangleDictionary was null!",-966,4,4);    
                                            }
                                            this.jsonparsestructure.Parsejson.CloseParsing(kutsuja+functionname,rememparseuidtwo.ToString());
                                        } else {
                                            this.prohmi.sendError(kutsuja+functionname,"Unexpected different amount of elements! Listelements:"+listtobeset.ReturnBoxList.Count()+" Dictionaryelements:"+amo,-963,4,4);
                                        }
                                    }
                                } else {
                                    this.prohmi.sendError(kutsuja+functionname,"Unexpected zero!",-962,4,4);
                                }
                            } else {
                                this.prohmi.sendError(kutsuja+functionname,"SmallRectangleDictionary was null!",-963,4,4);
                            }
                            this.jsonparsestructure.Parsejson.CloseParsing(kutsuja+functionname,rememparseuid.ToString());

                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"MotherconnectionJsonObj was null!",-964,4,4);
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Parsestruct was null!",-961,4,4);
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1146,4,4);
            }              
        }

        /// <summary>
        /// Tämä metodi palauttaa tämän objektin tallennuksessa tarvittavat tiedot JSON objektina
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {string} Palauttaa JSON objekti tyyppisen kokonaisuuden tekstijonona </returns>
        public string ReturnThisObjectAndSubobjectsParametersAsJSONForSaving(string kutsuja)
        {
            string retVal="";
            string functionname="->(CRS)ReturnThisObjectparameterAsJSONForSaving";

            if (this.classiniokay.IsClassInitialized==true) {
                retVal=this.ReturnThisObjectAndSubobjectsParametersAsJSON(kutsuja+functionname,ParamNameLists.ConnectionRectanglesSavingParamNames);
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1147,4,4);
            }

            return retVal;
        }

        /// <summary>
        /// Tämä metodi palauttaa tämän objektin tiedot JSON objektina
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="connrectanglesparamnames">List string tyyppisen listan referenssi, jossa on kaikki parametrinimet jotka halutaan printattavan JSON objektiin</param>
        /// <returns> {string} Palauttaa JSON objekti tyyppisen kokonaisuuden tekstijonona </returns>
        public string ReturnThisObjectAndSubobjectsParametersAsJSON(string kutsuja, List<string> connrectanglesparamnames)
        {
            string retVal="";
            string functionname="->(CRS)ReturnThisObjectparameterAsJSON";

            if (this.classiniokay.IsClassInitialized==true) {
                if (connrectanglesparamnames!=null) {
                    if (connrectanglesparamnames.Count>0) {
                        this.parameterprinter.SetConnectionRectanglesObjectToPrint(this);
                        retVal=this.parameterprinter.MyOwnParamPrint(kutsuja+functionname,connrectanglesparamnames,(int)ParamPrinter.myOwnTypePrintingEnum.JSON_OBJECT_WITH_PARAM_NAMES_AND_VALUES_2);
                    } else {
                        this.prohmi.sendError(kutsuja+functionname,"Parameter list didn't contain any parameter name (JSONparameters)!",-895,4,4);
                        retVal="ERROR=-110";
                    }
                } else {
                    this.prohmi.sendError(kutsuja+functionname,"Parameter list was null (JSONparameters)!",-896,4,4);
                    retVal="ERROR=-111";
                }
            } else {
                this.prohmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1148,4,4);
            }             
            return retVal;
        }
    }

    /// <summary>
    /// Tämä luokka vastaa ConnectionRectangle luokan parametristen tietojen säilytyksestä
    /// </summary>
    public class ConnectionRectangleData
    {
        /// <summary> ConnectionRectanglen UID, jonka alta tämä instanssi löytyy </summary>
        public long ParentUID { get; set; }

        /// <summary> Tämä tiedonsäilytysinstanssin oma UID </summary>
        public long OwnUID { get; set; }

        public ConnectionRectangleData(string kutsuja, long parentuid, long ownuid)
        {
            this.ParentUID=parentuid;
            this.OwnUID=ownuid;
        }     
    }

    /// <summary>
    /// Tämä luokka pitää sisällään ConnectionRectangle metodista sen graafisten komponenttien käsittelyyn liittyvät käskyt ja propertyt
    /// </summary>
    public class ConnectionRectangleUIComponents
    {
        /// <summary> "Äiti" laatikon uniqrefnum, eli ConnectionRectangle instanssin UID </summary>
        public long ParentUID { get; set; }

        /// <summary> Tämän luokan instanssin oma UID </summary>
        public long OwnUID { get; set; }

        /// <summary> Varsinainen Rectangle objekti </summary>
        public Rectangle myRect;

        /// <summary>
        /// Varsinainen tekstiblokki, johon RectangleText asetetaan
        /// </summary>
        public TextBlock rectTextBlock;

        /// <summary>
        /// Käyttöliittymän referenssi
        /// </summary>
        private ProgramHMI programhmi;

        /// <summary>
        /// Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille ja jolle voidaan rekisteröidä objektien referenssit
        /// </summary>
        private ObjectIndexer objindexerref;

        /// <summary>
        /// Tämän luokan instanssi huolehtii, että objekti on oikealla tavalla initialisoitu ja toimii, kuten olettaa saattaa
        /// </summary>
        private InitializationCounterClass classiniokay;

        /// <summary>
        /// Luku, joka täytyy YLITTÄÄ, jotta luokka katsotaan initialisoiduksi oikein
        /// </summary>
        private int mustexceedtreshold=0;

        /// <summary>
        /// Tämä luokka pitää sisällään ConnectionRectangle metodista sen graafisten komponenttien käsittelyyn liittyvät käskyt ja propertyt
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"> long, "Äiti" laatikon uniqrefnum, eli ConnectionRectangle instanssin UID </param>
        /// <param name="ownuid"> long, Tämän luokan instanssin oma UID </param>
        /// <param name="programi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="objindexer"> ObjectIndexer, Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille ja jolle voidaan rekisteröidä objektien referenssit </param>
        /// <returns>{void}</returns>
        public ConnectionRectangleUIComponents(string kutsuja, long parentuid, long ownuid, ProgramHMI programi, ObjectIndexer objind)
        {
            string functionname="->(CRUIC)ConnectionRectangleUICOmponents";
            this.ParentUID=parentuid;
            this.OwnUID=ownuid;
            this.programhmi=programi;
            this.objindexerref=objind;

            this.classiniokay=new InitializationCounterClass(kutsuja+functionname,this.mustexceedtreshold);

            if (this.objindexerref.objectlist.IndexOfKey(this.OwnUID)>-1) {
                this.classiniokay.AddClassOkayByNumber(kutsuja+functionname,1);
                int resp=this.objindexerref.SetObjectToIndexerWithErrorReport(kutsuja+functionname,this.OwnUID,this);
                if (resp<0) {
                    this.programhmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! Unsuccesful object set! UID:"+this.OwnUID+" Response:"+resp,-1127,4,4); 
                } else {
                    this.classiniokay.AddClassOkayByNumber(kutsuja+functionname,1);
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error! Problem to set object to objectindexer! No entry found! UID:"+this.OwnUID,-1128,4,4);
            }
        }

        /// <summary> Tämä metodi yksittäisen laatikon sijainnin, kun isoa laatikkoa on siirretty toiseen kohtaan vetämällä </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="newTop"> double, ison "äitilaatikon" uusi korkeusasema </param>
        /// <param name="newLeft"> double, ison "äitilaatikon" uusi leveysasema </param>
        /// <param name="letteroffsetleft"> double, laatikon teksitikirjaimen offset yhdyslaatikolle sen vasemmasta reunasta </param>
        /// <param name="letteroffsettop"> double, laatikon teksitikirjaimen offset yhdyslaatikolle sen yläreunasta </param>
        /// <returns> {int} Palauttaa 1, jos kohteen sijainnin päivitys onnistui. Jos -1=tuntematon virhe ja jos -3=ei asetettua laatikko-objektia päivitettäväksi, -4=luokkaa ei ole initialisoitu kunnolla </returns>
        public int UpdateRectangleUICompPositionsDuringDrag(string kutsuja, double newTop, double newLeft, double letteroffsetleft, double letteroffsettop)
        {
            int retVal=-1;
            string functionname="->(CRUIC)UpdateRectangleUICompPositionsDuringDrag";
            if (this.classiniokay.IsClassInitialized==true) {
                if (this.myRect!=null) {
                    Canvas.SetLeft(this.myRect, newLeft);
                    Canvas.SetTop(this.myRect, newTop); 
                    if (this.rectTextBlock!=null) {
                        Canvas.SetLeft(this.rectTextBlock, newLeft+letteroffsetleft);
                        Canvas.SetTop(this.rectTextBlock, newTop+letteroffsettop);
                    }               
                    retVal=1;
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"No Rectangle set! OwnUID:"+OwnUID+" ParentUID:"+ParentUID,-1085,4,4);
                    retVal=-3;
                }
            } else {
                retVal=-4;
                this.programhmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1177,4,4);
            }             
            return retVal;
        }

        /// <summary>
        /// Tämä metodi poistaa yhden rectanglen UI komponentit
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="objind"> ObjectIndexer, objectindexerin referenssi, jolla poistetaan kohde UID rekisteröinnin listoilta </param>
        /// <param name="origcanvas"> Canvas, se canvas josta kyseinen laatikko poistetaan</param>
        /// <param name="listofallboxesrefer"> SortedList long, ConnectionRectangle Lista kaikista laatikoista, joita on rekisteröity </param>
        /// <returns> {int} Palauttaa 1, jos sai poistettua Rectanglen UI komponentit. Jos palauttaa pienempi kuin 0, niin virhe! </returns></returns>
        public int RemoveRectangleUIComponents(string kutsuja, ObjectIndexer objind, Canvas origcanvas, SortedList<long, ConnectionRectangle> listofallboxesrefer)
        {
            string functionname="->(CRUIC)RemoveRectangleUIComponents";
            int retVal=-1;
            long rectnum=-1;
            int resp=-1;

            if (this.classiniokay.IsClassInitialized==true) {
                if (myRect!=null) {
                    if (origcanvas!=null) {
                        if (objind!=null) {

                            // Poistetaan rectangle canvasista
                            rectnum=(long)this.myRect.Tag;
                            if (rectnum>=0) {
                                // Poistetaan rectanglen teksti canvasista
                                origcanvas.Children.Remove(rectTextBlock);
                                this.rectTextBlock=null;

                                origcanvas.Children.Remove(myRect); // Poistetaan laatikko canvasista
                                myRect=null; // Asetetaan referenssi null:ksi
                                listofallboxesrefer.Remove(rectnum); // Poistetaan kohde kaikkien laatikoiden tagit sisältämässä listassa
                                resp=objind.DeleteObjectFromIndexer(kutsuja+functionname,rectnum); // Poistetaan rectanglen yksilöllinen tieto ObjectIndexeristä
                                retVal=1;
                                if (resp<1) {
                                    this.programhmi.sendError(kutsuja+functionname,"Error during delete operation in ObjectIndexer! Response:"+resp+" ObjectTag:"+rectnum+" OwnUID:"+OwnUID+" ParentUID:"+ParentUID,-743,4,4);
                                    retVal=-6; 
                                }
                            } else {
                                this.programhmi.sendError(kutsuja+functionname,"Rectangle didn't contain UID number! Response:"+rectnum+" OwnUID:"+OwnUID+" ParentUID:"+ParentUID,-742,4,4);
                                retVal=-5;                        
                            }
                        } else {
                            this.programhmi.sendError(kutsuja+functionname,"Object Indexer reference was null! Response:"+rectnum+" OwnUID:"+OwnUID+" ParentUID:"+ParentUID,-1088,4,4);
                            retVal=-7;
                        }
                    } else {
                        this.programhmi.sendError(kutsuja+functionname,"Canvas not set! OwnUID:"+OwnUID+" ParentUID:"+ParentUID,-741,4,4);
                        retVal=-4;
                    }
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"No Rectangle set! OwnUID:"+OwnUID+" ParentUID:"+ParentUID,-750,4,4);
                    retVal=-3;
                }           
            } else {
                retVal=-8;
                this.programhmi.sendError(kutsuja+functionname,"Error with class initialisation! Initialisation number:"+this.classiniokay.ClassOkayNumber+" when treshold:"+this.classiniokay.TresholdNumber,-1178,4,4);
            }               
            return retVal;
        }
    }

    /// <summary> Tämä luokka pitää sisällään yhden yksittäisen laatikon tiedot </summary>
    public class ConnectionRectangle
    {
        /// <summary> "Äiti" laatikon vanhemman uniqrefnum </summary>
        public long GranParentUID { get; set; }

        /// <summary> "Äiti" laatikon uniqrefnum, jonka alla nämä pikkulaatikot sijaitsevat </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Tämän objektin latauksen yhteydessä löydetty ParentUID tallennettuna, koska muuten voi syntyä ristiriitoja yksilöllisten tunnusten kanssa.
        /// </summary>
        public long LoadedParentUID { get; set; }

        /// <summary> Laatikon oma uniqrefnum </summary>
        public long OwnUID { get; set; }

        /// <summary>
        /// Tämän objektin latauksen yhteydessä löydetty OwnUID tallennettuna, koska muuten voi syntyä ristiriitoja yksilöllisten tunnusten kanssa.
        /// </summary>
        public long LoadedOwnUID { get; set; }

        /// <summary> Laatikosta johonkin toiseen kohteeseen lähtevien yhteyksien uniqrefnumit latauksen yhteydessä, koska muuten voi syntyä tunnus ristiriitoja </summary> 
        public List<long> LoadedConnectionUIDs;

        /// <summary> Laatikosta johonkin toiseen kohteeseen lähtevien yhteyksien uniqrefnumit </summary> 
        public List<long> ConnectionUIDs;               

        private string rectangletext="";
        /// <summary> Teksti, joka kirjoitetaan pikkurectanglen yläosaan </summary>
        public string RectangleText { 
            get { return this.rectangletext; } 
            set { 
                this.rectangletext=value;
                if (this.connrectuicomps.rectTextBlock!=null) {
                    this.connrectuicomps.rectTextBlock.Text=this.rectangletext;
                }
            }
        }

        /// <summary> Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </summary>
        private ObjectIndexer objindexerref;

        /// <summary> Käyttöliittymäluokan referenssi </summary>
        private ProgramHMI programhmi;   

        /// <summary> Referenssi Connectionshandler luokkaan, joka pitää connection objektit järjestyksessä listassa </summary>
        private ConnectionsHandler connecthandler;

        /// <summary>
        /// Referenssi ParamPrinter luokan instanssiin, joka hoitaa objektin komponenttien printtauksesta haluttuun muotoon
        /// </summary>
        private ParamPrinter paramprint;

        /// <summary>
        /// Parametristen grafiikasta erotettujen tietojen säilyttämiseen oma ConnectionRectangleData luokan instanssi
        /// </summary>
        private ConnectionRectangleUIComponents connrectuicomps;

        /// <summary>
        /// Referenssi Canvasiin, johon tämä connection rectanglen graafinen komponentti oheiskomponentteineen on asetettu
        /// </summary>
        private int createuicomponents=(int)ImportantProgramParams.createUIComponents.DO_NOT_CREATE_UI_COMPONENTS_0;

        /// <summary>
        /// Tämä muuttuja pitää sisällään tämän rectanglen oman indekserin connectionUIDs listaan ja First käsky nollaa tämän ensimmäiseen elementtiin ja Next käsky liikuttaa yhdellä aina seuraavaan elementtiin
        /// </summary>
        private int connectionuidsnextlistindex=-1;
        
        /// <summary>
        /// Tämä BlockAtomValue saa jossain vaiheessa tiedon, jonka Connection on tarjoillut sille. Tätä receivingatomvalue:ta voidaan sitten käyttää esim. OperationBlock tyyppisille blokeille testauttamaan onko tieto ohjautunut tänne saakka
        /// </summary> 
        private BlockAtomValue receivingatomvalue;

        /// <summary>
        /// Tämä BlockAtomValue saa jossain vaiheessa tiedon, jonka Connection on tarjoillut sille. Tätä receivingatomvalue:ta voidaan sitten käyttää esim. OperationBlock tyyppisille blokeille testauttamaan onko tieto ohjautunut tänne saakka
        /// </summary> 
        public BlockAtomValue BlockAtomValueRef {
            get { return this.receivingatomvalue; }
        } 

        /// <summary>
        /// Tämä UID arvo viittaa suoraan Blokin kahvan UID arvoon, johon tämä ConnectionRectangle on liitetty. Jos tämä property on -1, niin liitosta ei ole tehty ja voidaan pitää virheellisenä toimintana
        /// </summary> 
        public long BlockHandleUIDForReference { get; set; }

        /// <summary> Constructor - luo objektin instanssin, joka pitää sisällään ison sinisen laatikon sisälle tulevat pikkulaatikot </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"> long, "äiti" laatikon uniqrefnum jonka mukana nämä pikkulaatikot liikkuvat </param>
        /// <param name="granparentuid"> long, "Äiti" laatikon vanhemman uniqrefnum </param>
        /// <param name="objindexer"> ObjectIndexer, Referenssi object indexeriin, jolta saadaan tarvittavat uniqrefnum:it objekteille </param>
        /// <param name="progrhmi"> ProgramHMI, referenssi käyttöliittymäluokkaan </param>
        /// <param name="parpri"> Param Printer, Referenssi ParamPrinter luokan instanssiin, joka hoitaa objektin komponenttien printtauksesta haluttuun muotoon </param>
        /// <param name="connhandle"> ConnectionsHandler, referenssi luokkaan, joka pitää connection objektit järjestyksessä listassa </param>
        /// <param name="recttext"> string, teksti, joka kirjoitetaan yhdistysrectanglen yläosaan </param>
        /// <param name="createdownuid"> out long, palauttaa metodin itsensä luoman uid arvon </param>
        /// <param name="creategraphcomp"> int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </param>
        /// <returns> {void} </returns>
        public ConnectionRectangle(string kutsuja, long parentuid, long granparentuid, ObjectIndexer objindexer, ProgramHMI progrhmi, ParamPrinter parpri, ConnectionsHandler connhandle, string recttext, out long createdownuid, int creategraphcomp=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1)
        {
            string functionname="->(CR)Constructor";
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.BlockHandleUIDForReference=-1;
            this.objindexerref=objindexer;
            this.programhmi=progrhmi;
            this.connecthandler=connhandle;
            //this.paramprint=parpri;
            this.paramprint = new ParamPrinter(kutsuja+functionname, this.programhmi, parpri.merkkijonokasittelija);
            createdownuid=-10;
            this.OwnUID = this.objindexerref.AddObjectToIndexer(kutsuja+functionname,parentuid,(int)ObjectIndexer.indexerObjectTypes.CONNECTION_RECTANGLE_100,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.GranParentUID); // Tässä -1 on objectindexarrayn tieto, jota ei ole määritelty
            if (this.OwnUID<0) {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+this.OwnUID+" ParentUid:"+parentuid,-1061,4,4);                
            } else {
                createdownuid=this.OwnUID;
                if (this.objindexerref.objectlist.IndexOfKey(this.OwnUID)>-1) {
                    int resp=this.objindexerref.SetObjectToIndexerWithErrorReport(kutsuja+functionname,this.OwnUID,this);
                    if (resp<0) {
                        this.programhmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! Unsuccesful object set! UID:"+this.OwnUID+" Response:"+resp,-1125,4,4); 
                    }
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"Fatal error! Problem to set object to objectindexer! No entry found! UID:"+this.OwnUID,-1126,4,4);
                }
            }
            
            long subuid = this.objindexerref.AddObjectToIndexer(kutsuja+functionname,this.OwnUID,(int)ObjectIndexer.indexerObjectTypes.CONNECTION_RECTANGLE_UI_COMPONENTS_151,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1, this.ParentUID); // Tässä -1 on objectindexarrayn tieto, jota ei ole määritelty
            if (subuid>=0) {
                this.connrectuicomps = new ConnectionRectangleUIComponents(kutsuja+functionname,this.OwnUID,subuid,this.programhmi,this.objindexerref); // Luodaan tietojen säilyttämiseen ConnectionRectangleData luokan instanssi
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+subuid+" ParentUid:"+parentuid,-1062,4,4);
                this.connrectuicomps=null;
            }            
            this.RectangleText=recttext;
            this.createuicomponents=creategraphcomp;

            this.ConnectionUIDs = new List<long>();
            this.LoadedConnectionUIDs = new List<long>();

            this.receivingatomvalue = new BlockAtomValue();
        }

        /// <summary> Tämä property palauttaa varsinaisen luodun rectanglen referenssin </summary>
        public Rectangle RectangleObject {
            get {
                if (this.createuicomponents!=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                    this.programhmi.sendError("->(CR)RectangleObject","UI components not created! Response:"+this.createuicomponents,-1038,4,4);
                    return null;
                } else {
                    return this.connrectuicomps.myRect; 
                }
            }
            set {
                if (this.createuicomponents!=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                    this.programhmi.sendError("->(CR)RectangleObject","UI components not created! Response:"+this.createuicomponents,-1039,4,4);
                } else {                 
                    this.connrectuicomps.myRect=value;
                }
            }
        }

        /// <summary> Tämä property palauttaa varsinaisen luodun rectanglen tekstilaatikon referenssin </summary>
        public TextBlock RectangleObjectText {
            get {
                if (this.createuicomponents!=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                    this.programhmi.sendError("->(CR)RectangleObjectText","UI components not created! Response:"+this.createuicomponents,-1040,4,4);
                    return null;
                } else {
                    return this.connrectuicomps.rectTextBlock; 
                }
            }
            set {
                if (this.createuicomponents!=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                    this.programhmi.sendError("->(CR)RectangleObjectText","UI components not created! Response:"+this.createuicomponents,-1041,4,4);
                } else {                
                    this.connrectuicomps.rectTextBlock=value;
                    this.connrectuicomps.rectTextBlock.Text=this.RectangleText;
                }
            }
        }

        /// <summary>
        /// Kopioi ConnectionRectangle-luokan BlockAtomValue-arvot BlockHandle-luokan instanssin BlockAtomValue-arvoihin.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <returns>Palauttaa 1, jos toimenpide onnistui. Palauttaa 0, jos ulkoinen referenssi on käytössä. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos toimenpide epäonnistui.</returns>
        public int CopyBlockAtomValueToBlockHandle(string kutsuja)
        {
            string functionname = "->(CR)CopyBlockAtomValueToBlockHandle";
            
            if (this.BlockHandleUIDForReference >= 0) {
                BlockHandle blockHandle = this.objindexerref.GetTypedObject<BlockHandle>(kutsuja + functionname, this.BlockHandleUIDForReference);
                
                if (blockHandle != null) {
                    if (blockHandle.ReturnBlockAtomValueRef != null) {
                        if (blockHandle.ReturnBlockAtomValueRef != this.receivingatomvalue) {
                            blockHandle.ReturnBlockAtomValueRef.CopyFrom(this.receivingatomvalue);
                            return 1;
                        } else {
                            return 0; // Ulkoinen referenssi on käytössä
                        }
                    } else {
                        this.programhmi.sendError(kutsuja + functionname, "BlockHandle's BlockAtomValue reference is null", -1322, 4, 4);
                        return -1;
                    }
                } else {
                    this.programhmi.sendError(kutsuja + functionname, "Failed to retrieve BlockHandle", -1323, 4, 4);
                    return -2;
                }
            } else {
                this.programhmi.sendError(kutsuja + functionname, "Invalid BlockHandleUIDForReference! Response:"+this.BlockHandleUIDForReference, -1324, 4, 4);
                return -3;
            }
        }

        /// <summary>
        /// Tämä käsky ottaa tähän ConnectionRectangleen liittyvän ConnectionUIDs listan käsittelyyn ja palauttaa Connection objektien UID arvoista ensimmäisen
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns>{long}, palauttaa ConnectionUIDs listasta Connection objektien ensimmäisen kohteen UID arvon. Jos virhe, palauttaa pienemmän kuin 0. -1=jos epämääräinen virhe, -2=jos listalla ei ollut yhtään kohdetta.</returns> 
        public long IterateThroughConnectionsFirst(string kutsuja)
        {
            long retVal=-1;
            if (this.ConnectionUIDs.Count()>0) {
                retVal=this.ConnectionUIDs.ElementAt(0);
                this.connectionuidsnextlistindex=0;
            } else {
                retVal=-2;
            }
            return retVal;
        }

        /// <summary>
        /// Returns the first Connection object from the ConnectionUIDs list.
        /// </summary>
        /// <param name="kutsuja">The path of the caller invoking this function.</param>
        /// <returns>{Connection} Returns the first Connection object if successful, null if the list is empty or an error occurs.</returns>
        public Connection IterateThroughConnectionsFirstReturnConnectionObject(string kutsuja)
        {
            string functionname = "->(CR)IterateThroughConnectionsFirstReturnConnectionObject";
            if (ConnectionUIDs.Any()==true)
            {
                long firstUID = ConnectionUIDs.ElementAt(0);
                this.connectionuidsnextlistindex = 0;
                return this.ReturnConnectionByUID(kutsuja, firstUID);
            } else {
                this.programhmi.sendError(kutsuja + functionname, "ConnectionUIDs list is empty.", -1135, 4, 4);
                return null;
            }
        }

        /// <summary>
        /// Tämä käsky ottaa tähän ConnectionRectangleen liittyvän ConnectionUIDs listan käsittelyyn ja palauttaa Connection objektien UID arvot yksi kerrallaan
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="currentconnectionuid"> long, Connectionin UID, jota halutaan etsiä ConnectionUIDs listalta ja antaa kyseisestä komponentista seuraavan kohteen UID. Jos -1, niin käytetään luokan omaa indeksilaskuria</param>
        /// <returns>{long}, palauttaa ConnectionUIDs listasta Connection objektin seuraavan UID arvon. Jos virhe, palauttaa pienemmän kuin 0. -10=jos epämääräinen virhe, -2=jos listan kohteissa mentiin liian pitkälle, eikä listassa ole seuraavaa, -3=jos annettiin tämänhetkinen UID ja sitä ei löytynyt listalta</returns> 
        public long IterateThroughConnectionsNext(string kutsuja, long currentconnectionuid=-1)
        {
            string functionname="->(CR)IterateThroughConnectionsNext";
            long retVal=-10;
            int i;
            if (currentconnectionuid>=0) {
                i=this.ConnectionUIDs.IndexOf(currentconnectionuid);
                if (i==-1) {
                    this.programhmi.sendError(kutsuja+functionname,"Couldn't find connection with given currentconnectionuid! UID:"+currentconnectionuid,-1131,4,4);
                    retVal=-3;
                    return retVal;
                }
            } else {
                i=this.connectionuidsnextlistindex;
            }

            i++;

            if (i<this.ConnectionUIDs.Count) {
                retVal=this.ConnectionUIDs.ElementAt(i);
            } else {
                retVal=-2;
            }
            this.connectionuidsnextlistindex=i;            
            return retVal;
        }



        /// <summary>
        /// Iterates through the ConnectionUIDs list and returns the next Connection object.
        /// </summary>
        /// <param name="kutsuja">The path of the caller invoking this function.</param>
        /// <param name="currentconnectionuid">Current Connection UID for finding the next one. If -1, uses the class's index counter.</param>
        /// <returns>{Connection} Returns the next Connection object if found, null if there is an error or end of the list.</returns>
        public Connection IterateThroughConnectionsNextReturnConnectionObject(string kutsuja, long currentconnectionuid = -1)
        {
            string functionname = "->(CR)IterateThroughConnectionsNextReturnConnectionObject";
            long nextUID=this.IterateThroughConnectionsNext(kutsuja+functionname,currentconnectionuid);

            if (nextUID >= -2) {
                if (nextUID<0) {
                    return null;
                } else {
                    return ReturnConnectionByUID(kutsuja, nextUID);
                }
            } else {
                this.programhmi.sendError(kutsuja + functionname, "Error during returning Connection object! Response:"+nextUID, -1136, 4, 4);
                return null;
            }
        }


        /// <summary>
        /// Tietämällä Connection objektin UID:n, tämä funktio palauttaa kohteen objektin
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <returns>{Connection}, palauttaa löytmänsä Connection objektin tai null, mikäli objektin etsinnässä tuli virhe</returns>
        public Connection ReturnConnectionByUID(string kutsuja, long UIDtoseek)
        {
            string functionname="->(CR)ReturnConnectionWithUID";
            if (this.objindexerref.objectlist.IndexOfKey(UIDtoseek)>-1) {
                Connection connobj;
                if (this.objindexerref.objectlist[UIDtoseek].ObjectRef!=null) {
                    var conno = this.objindexerref.objectlist[UIDtoseek].ObjectRef;
                    connobj = conno as Connection;
                    if (connobj!=null) {
                        return connobj;
                    } else {
                        this.programhmi.sendError(kutsuja+functionname,"Failed to cast the object to a Connection type!! UID:"+UIDtoseek,-1132,4,4);
                        return null;
                    }
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"Problem with objectindexer! Object wasn't set in objectindexer and we found null! UID:"+UIDtoseek,-1133,4,4);
                    return null;
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"No object found with the specified UID! UID:"+UIDtoseek,-1134,4,4);
                return null;
            }
        }

        /// <summary>
        /// Tämä metodi palauttaa tämän objektin tallennuksessa tarvittavat tiedot JSON objektina tallennusta varten
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {string} Palauttaa JSON objekti tyyppisen kokonaisuuden tekstijonona </returns>
        public string ReturnThisObjectParametersAsJSONForSaving(string kutsuja)
        {
            string retVal="";
            string functionname="->(CR)ReturnThisObjectparameterAsJSONForSaving";
            retVal=this.ReturnThisObjectParametersAsJSON(kutsuja+functionname,ParamNameLists.ConnectionRectangleSavingParamNames);
            return retVal;
        }

        /// <summary>
        /// Tämä metodi palauttaa tämän objektin tallennuksessa tarvittavat tiedot JSON objektina
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="connrectangleparamnames">List string tyyppisen listan referenssi, jossa on kaikki parametrinimet jotka halutaan printattavan JSON objektiin</param>
        /// <returns> {string} Palauttaa JSON objekti tyyppisen kokonaisuuden tekstijonona </returns>
        public string ReturnThisObjectParametersAsJSON(string kutsuja, List<string> connrectangleparamnames)
        {
            string retVal="";
            string functionname="->(CR)ReturnThisObjectparameterAsJSON";
            if (connrectangleparamnames!=null) {
                if (connrectangleparamnames.Count>0) {
                    this.paramprint.SetConnectionRectangleObjectToPrint(this);
                    retVal=this.paramprint.MyOwnParamPrint(kutsuja+functionname,connrectangleparamnames,(int)ParamPrinter.myOwnTypePrintingEnum.JSON_OBJECT_WITH_PARAM_NAMES_AND_VALUES_2);
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"Parameter list didn't contain any parameter name (JSONparameters)!",-893,4,4);
                    retVal="ERROR=-110";
                }
            } else {
                this.programhmi.sendError(kutsuja+functionname,"Parameter list was null (JSONparameters)!",-894,4,4);
                retVal="ERROR=-111";
            }            
            return retVal;
        }

        /// <summary> Tämä metodi poistaa yksittäisen yhdistyslaatikon osalta yhdyslaatikon itsensä ylläpitämän tiedon yhteyksistään. HUOM! Tämä käsky ei vielä poista  </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="removingUID"> long, poistettavan kohteen uniqrefnum, joka on tarkoitus poistaa ConnectionUIDs listalle kerätyistä Connectioneiden uniqrefnumeista </param>
        /// <returns> {int} 1=jos poisto onnistui, 0=jos kohdetta ei poistettavalla numerolla ollut listassa ja -2=jos yhtään kohdetta ei ollut listassa </returns>
        public int RemoveConnectionUID(string kutsuja, long removingUID)
        {
            int i=0;
            int amo=ConnectionUIDs.Count;
            int retVal=0;

            if (amo>0) {
                while (i<amo) {
                    if (this.ConnectionUIDs.ElementAt(i)==removingUID) {
                        this.ConnectionUIDs.RemoveAt(i);
                        amo=ConnectionUIDs.Count;
                        retVal=1;
                    } else {
                        i++;
                    }
                }
            } else {
                retVal=-2;
            }
            return retVal;
        }   

        /// <summary> Tämä metodi yksittäisen laatikon sijainnin, kun isoa laatikkoa on siirretty toiseen kohtaan vetämällä </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="newTop"> double, ison "äitilaatikon" uusi korkeusasema </param>
        /// <param name="newLeft"> double, ison "äitilaatikon" uusi leveysasema </param>
        /// <param name="letteroffsetleft"> double, laatikon teksitikirjaimen offset yhdyslaatikolle sen vasemmasta reunasta </param>
        /// <param name="letteroffsettop"> double, laatikon teksitikirjaimen offset yhdyslaatikolle sen yläreunasta </param>
        /// <returns> {int} Palauttaa 1, jos kohteen sijainnin päivitys onnistui. Jos -1=tuntematon virhe ja jos -3=ei asetettua laatikko-objektia päivitettäväksi </returns>
        public int UpdatePositionsDuringDrag(string kutsuja, double newTop, double newLeft, double letteroffsetleft, double letteroffsettop)
        {
            int retVal=-1;
            string functionname="->(CR)UpdatePositionsDuringDrag";

            if (this.createuicomponents==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                retVal=this.connrectuicomps.UpdateRectangleUICompPositionsDuringDrag(kutsuja+functionname,newTop,newLeft,letteroffsetleft,letteroffsettop);
                if (retVal<1) {
                    this.programhmi.sendError(kutsuja+functionname,"Rectangle positions update failed! Response:"+retVal+" CreateUI:"+this.createuicomponents,-1086,4,4);
                } else {
                    if (this.ConnectionUIDs.Count>0) { // Jos kohteeseen on kytketty eri connectioneja, niin päivitetään myös ne
                        foreach(long updateposuid in this.ConnectionUIDs) {
                            retVal=this.connecthandler.UpdateConnectionOnCanvasWithUID(kutsuja+functionname,updateposuid);
                            if (retVal<1) {
                                this.programhmi.sendError(kutsuja+functionname,"Rectangle connection positions update failed! Response:"+retVal+" CreateUI:"+this.createuicomponents+" UpdateConnUID:"+updateposuid,-1087,4,4);
                            }
                        }
                    }
                }
            } else {
                retVal=1; // Palautetaan 1, jos ei tarvitse tehdä mitään
            }

            return retVal;
        }

        /// <summary>
        /// Tämä metodi poistaa yhden rectanglen ja siihen liittyvät connectionit
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="origcanvas"> Canvas, se canvas josta kyseinen laatikko poistetaan</param>
        /// <param name="listofallboxesrefer"> SortedList long, ConnectionRectangle Lista kaikista laatikoista, joita on rekisteröity </param>
        /// <returns> {int} Palauttaa 1, jos sai poistettua Rectanglen komponentit sekä Connectionit, kuten myös niiden UI:n vastaavuudet. Jos palauttaa pienempi kuin 0, niin virhe! </returns>
        public int RemoveRectangle(string kutsuja, Canvas origcanvas, SortedList<long, ConnectionRectangle> listofallboxesrefer)
        {
            string functionname="->(CR)RemoveRectangle";
            int retVal=-1;
            int i=0;
            long removeposuid=-1;
            int amok=-1;

            amok=this.ConnectionUIDs.Count;
            if (amok>0) { // Jos kohteeseen on kytketty eri connectioneja, niin päivitetään myös ne

                while (i<amok) {
                    removeposuid=this.ConnectionUIDs.ElementAt(i);
                    
                    this.connecthandler.RemoveSingleConnection(kutsuja+functionname,removeposuid,origcanvas,listofallboxesrefer);

                    this.ConnectionUIDs.Remove(removeposuid);
                    amok=this.ConnectionUIDs.Count;
                }
            }

            this.rectangletext="";

            if (this.createuicomponents==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                if (this.connrectuicomps!=null) {
                    retVal=this.connrectuicomps.RemoveRectangleUIComponents(kutsuja+functionname,this.objindexerref,origcanvas,listofallboxesrefer);                    
                } else {
                    this.programhmi.sendError(kutsuja+functionname,"Connection Rectangle UI Components reference was null! OwnUID:"+OwnUID+" ParentUID:"+ParentUID,-1089,4,4);
                    retVal=-40;
                }
            } else {
                retVal=1; // Jos ei tarvitse tuhota UI komponetteja, niin palautetaan vain 1
            }
            return retVal;           
        }
    }
