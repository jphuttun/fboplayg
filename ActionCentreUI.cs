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
using Accessibility;

    /// <summary>
    /// Tämä luokka pitää sisällään ActionCentreUI luokan alta löytyvän blokkikonstruktion referenssit, kuten listofmotherboxes ja listofallboxesreferences sekä connectionhandlerin referenssin. 
    /// Näitä hyödyntämällä päästään käsiksi blokkeihin, joita voidaan ajaa yksi kerrallaan, jotta ohjelma saadaan tekemään haluttuja asioita.
    /// </summary>
    public class ActionCentreConstructionHandler
    {
        /// <summary> Käyttöliittymäluokan referenssi </summary>
        private ProgramHMI proghmi;

        /// <summary> ConnectionHandler luokka ylläpitää listaa kaikista Connection objekteista </summary> 
        private ConnectionsHandler connhandler;

        /// <summary> Referenssi ObjectIndexer luokkaan, joka jakaa uniqrefnumit (eli UID) tiedot jokaiselle ohjelmaan luotavalle pysyvälle objektille </summary>
        private ObjectIndexer objindexerref;        

        /// <summary> Lista kaikista päälaatikoista (siniset), joiden alla on sitten pikkulaatikot </summary>
        public SortedList<long, MotherConnectionRectangle> listofMotherBoxes;

        /// <summary> Tässä listassa on kaikki ConnectionRectangle kohteiden UID avaimena ja referenssi itse kohteeseen </summary>
        public SortedList<long, ConnectionRectangle> listofAllBoxesReferences;

        /// <summary>
        /// Äitiobjektin UID, jolla saadaan tietoomme minkä äitobjekti ActionCentreUI:n alle tämä objektin instanssi on luotu
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Parent objektin äitiobjektin UID
        /// </summary>
        public long GranParentUID { get; set; }

        /// <summary>
        /// Meidän oma UID, jolla saadaan tietoomme tämän instanssin sijainti rekisteröidyssä rakenteessa
        /// </summary>
        public long OwnUID { get; set; }        

        /// <summary>
        /// Parametrisista toimintapoluista vastaavan luokan referenssi, jolla voi hyödyntää blokkirakenteen toimintaobjekteja. Lisäksi säilyttää toimintablokkien sisältöjä
        /// </summary>
        private ActionCentre actioncent;

        /// <summary>
        /// Tämän luokan instanssi pitää kirjaa initialisoinnista tämän kyseisen luokan kohdalla
        /// </summary>
        private InitializationCounterClass inicountc;

        /// <summary>
        /// Luo uuden instanssin ActionCentreConstructionHandler-luokasta.
        /// </summary>
        /// <param name="kutsuja">string, Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="parentuid">long, Äitiobjektin UID.</param>
        /// <param name="granparentuid">long, Parent objektin äitiobjektin UID.</param>
        /// <param name="prhmi">ProgramHMI, Käyttöliittymäluokan referenssi.</param>
        /// <param name="objind">ObjectIndexer, Referenssi ObjectIndexer luokkaan.</param>
        /// <param name="connhand">ConnectionsHandler, ConnectionsHandler luokan referenssi.</param>
        /// <returns> {void} </returns>
        public ActionCentreConstructionHandler(string kutsuja, long parentuid, long granparentuid, ProgramHMI prhmi, ObjectIndexer objind, ConnectionsHandler connhand)
        {
            string functionname = "->(ACCH)Constructor";
            this.ParentUID = parentuid;
            this.GranParentUID = granparentuid;
            this.proghmi = prhmi;
            this.objindexerref = objind;
            this.connhandler = connhand;
            this.inicountc = new InitializationCounterClass(kutsuja + functionname, 0); // Initialisointia valvovan luokan instanssi, joka valvoo tämän luokan initialisointivaiheita
            this.OwnUID = this.objindexerref.AddObjectToIndexer(kutsuja + functionname, this.ParentUID, (int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_CONSTRUCTION_HANDLER_320, -1, (int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1, this.GranParentUID); // Tässä jälkimmäinen -1 on objectindexarray
            this.listofMotherBoxes = new SortedList<long, MotherConnectionRectangle>();
            this.listofAllBoxesReferences = new SortedList<long, ConnectionRectangle>();
            if (this.OwnUID >= 0) {
                int respo = this.objindexerref.SetObjectToIndexerWithErrorReport(kutsuja + functionname, this.OwnUID, this, (int)ObjectIndexer.rewriteOldObjectReference.ALWAYS_REWRITE_OBJECT_REFERENCE_0);
                if (respo >= 0) {
                    this.inicountc.AddClassOkayByNumber(kutsuja + functionname, 1);
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Failed to set object to indexer with error report. Response: " + respo, -1265, 4, 4);
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "Failed to add object to indexer. OwnUID: " + this.OwnUID, -1266, 4, 4);
            }
        }

        /// <summary>
        /// Tätä metodia on kutsuttava constructorin lisäksi, jotta luokka on oikealla tavalla initialisoitu!
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="actcen"></param>
        /// <returns> {void} </returns>
        public void InitializeActionCentreConstructionHandler(string kutsuja, ActionCentre actcen)
        {
            string functionname="->(ACCH)InitializeActionCentreConstructionHandler";
            this.actioncent=actcen;
            this.inicountc.AddClassOkayByNumber(kutsuja+functionname,1); // Initialisointia valvovan luokan instanssi, joka valvoo tämän luokan initialisointivaiheita
        }

        /// <summary>
        /// Tämä metodi tarkistaa, löytyykö blockuid indeksillä MotherConnectionRectangle listofMotherBoxes listalta ja palauttaa referenssin kyseiseen objektiin
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="blockuid"> long, etsittävän MotherConnectionRectangle kohteen UID, jolla se on tallennettu listofMotherBoxes listaan</param>
        /// <param name="printerrors">bool, printataanko virheet, jos etsinnässä ei löytynyt kohteita tai niiden jälkitarkistuksessa tuli ongelmia</param>
        /// <param name="testaltroute">bool, testataanko onko kohteen RouteId parametrissa sama luku, kuin meillä tiedossa oleva altrouteval</param>
        /// <param name="blockaltrouteval">int, jos testaltroute parametri on true, niin testataan onko blockaltrouteval sama kuin objektin RouteId</param>
        /// <returns> {MotherConnectionRectangle}, palauttaa blockuid tunnuksella löydetyn kohteen referenssin tai null, jos kohdetta ei löytynyt tai sen etsinnässä tuli virhe! </returns>
        public MotherConnectionRectangle ReturnMainBlock(string kutsuja, long blockuid, bool printerrors, bool testaltroute=false, int blockaltrouteval=-1)
        {
            string functionname="->(ACCH)ReturnMainBlock";

            if (this.inicountc.IsClassInitialized==true) {
                MotherConnectionRectangle mothrect;
                if (this.listofMotherBoxes.IndexOfKey(blockuid)>-1) {
                    mothrect=this.listofMotherBoxes[blockuid];
                    if (testaltroute==true) {
                        if (mothrect.StoredUIcomps.StoredParamValues.RouteId==blockaltrouteval) {
                            return mothrect;
                        } else {
                            if (printerrors==true) {
                                this.proghmi.sendError(kutsuja+functionname,"Tested altroute didn't match! BlockUID:"+blockuid+" TestedRoute:"+blockaltrouteval,-1091,4,4);
                            }
                            return null;
                        }
                    } else {
                        return mothrect;
                    }
                } else {
                    if (printerrors==true) {
                        this.proghmi.sendError(kutsuja+functionname,"No object in MotherConnectionList with UID:"+blockuid,-1092,4,4);
                    }
                    return null;
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization steps:"+this.inicountc.ClassOkayNumber+" Initialization treshold:"+this.inicountc.TresholdNumber,-1097,4,4);
                return null;
            }
        }
    }

    /// <summary> Parametristen toimintapolkujen luontiin suunniteltu käyttöliittymä luokka </summary>
    public class ActionCentreUI
    {
        /// <summary> Luokka, jonka kautta käynnistetään erilliseen threadiin kaupankäyntiä suorittava ohjelmalooppi </summary>
        private ProgramStarter progstarter;

        /// <summary> BPActions luokan referenssi. Luokasta löytyy funktiot, joita ajetaan, kun painetaan jotain painiketta näppäimistöllä tai klikataan jotain tiettyä nappia. Osa toiminnoista voi olla vielä siirtämättä PrimaryCode luokasta </summary>
        private BPActions bpact;

        /// <summary> BPActions luokan referenssi. Luokasta löytyy funktiot, joita ajetaan, kun painetaan jotain painiketta näppäimistöllä tai klikataan jotain tiettyä nappia. Osa toiminnoista voi olla vielä siirtämättä PrimaryCode luokasta </summary>
        public BPActions ReturnBPActionsReference {
            get { return bpact; }
        }

        /// <summary> Käyttöliittymäluokan referenssi </summary>
        private ProgramHMI proghmi;

        /// <summary> Ensimmäinen valittu laatikko, josta viiva piirretään toiseen laatikkoon </summary>
        private Rectangle firstSelectedBox = null;

        /// <summary> Toinen valittu laatikko, johon viiva piirretään ensimmäisestä laatikosta firsSelectedBox </summary>        
        private Rectangle secondSelectedBox = null;

        /// <summary> Sisältää tiedon, ollaanko juuri paraikaa yhdistämässä kahta eri laatikkoa </summary>
        private bool isConnecting = false;

        /// <summary> Printtaa tiedon, että olemme painamassa yhdistyslaatikkoja ensimmäisen tai toisen kerran </summary>
        private TextBlock connectionStatusText;

        /// <summary> Painike jolla luodaan yhteydet laatikoiden välille. Koska painikkeen sisältöä täytyy muuttaa, täytyy painikkeen olla koko luokan jäsen. </summary>
        private Button connectButton;

        /// <summary> Combobox, jolla valitaan väri, millä värillä yhdysviivat laatikkojen väleille halutaan luoda </summary>
        private ComboBox colorComboBox;

        /// <summary> Väri, jota käytetään viivan piirtämiseen </summary>
        private Brush selectedColor = Brushes.Black;

        /// <summary> Printtaa tiedon, että olemme painamassa poista yhdistys laatikkoja ensimmäisen tai toisen kerran </summary>
        private bool isDisconnecting = false;

        /// <summary> Painike, jolla katkaistaan yhteys kahden laatikon väliltä. Koska painikkeen sisältöä täytyy muuttaa, täytyy painikkeen olla koko luokan jäsen. </summary> 
        private Button disconnectButton;
        
        /// <summary> Kertoo, onko poistotila aktiivinen </summary>
        private bool isDeleting = false; 
        
        /// <summary> Viittaus "Poista objekti" -nappiin </summary>
        private Button deleteButton; 
        
        /// <summary>
        /// Napin referenssi, jolla luodaan valintablokki ActionCentreUI:hin - valintablokissa on mahdollista valita ==, != jne.
        /// </summary>
        private Button lisaaValintaButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan between/outside valintablokki ActionCentreUI:hin - valintablokissa on mahdollista valita joko between tai outside testaus kahden luvun ja syötteen väliltä
        /// </summary>
        private Button betweenButton;

        /// <summary>
        /// Napin referenssi, jolla voidaan luoda lähtöarvo blokki ActionCentreUI:hin ja tuoda järjestelmän jokin parametri lähtöarvoksi
        /// </summary>
        private Button lahtoarvoButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan blokki ActionCentreUI:hin, jolla voi tehdä matemaattisen operaation kahden luvun kanssa - vihreä on se jolle operaatio tehdään - operaatioita ovat +,-,*,/
        /// </summary>
        private Button operaatio2Button;

        /// <summary>
        /// Napin referenssi, jolla luodaan blokki ActionCentreUI:hin, jolla voi tehdä matemaattisen operaation kolmen luvun kanssa - vihreä on se jolle operaatio tehdään ja keltaiset joita käytetään jakajana, miinustajana jne. - operaatioita ovat +,-,*,/
        /// </summary>
        private Button operaatio3Button;            

        /// <summary>
        /// Napin referenssi, jolla luodaan blokki ActionCentreUI:hin, jolla atomeista voi kasata isompaa masterblokkia. Tällöin KahvaIn blokit ovat lähtöarvoja masterBlokille ja kahvaOut blokit ovat tietoja joita palautetaan masterblokista ulospäin
        /// </summary>
        private Button kahvaInButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan blokki ActionCentreUI:hin, jolla atomeista voi kasata isompaa masterblokkia. Tällöin KahvaIn blokit ovat lähtöarvoja masterBlokille ja kahvaOut blokit ovat tietoja joita palautetaan masterblokista ulospäin
        /// </summary>
        private Button kahvaOutButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan ActionCentreUI:hin Market Buy operaation kauppapaikassa halutulla markkinalla tekevän objektin
        /// </summary>
        private Button marketBuyButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan ActionCentreUI:hin Market Sell operaation kauppapaikassa halutulla markkinalla tekevän objektin
        /// </summary>
        private Button marketSellButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan ActionCentreUI:hin Limit Buy operaation kauppapaikassa halutulla markkinalla tekevän objektin
        /// </summary>
        private Button limitBuyButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan ActionCentreUI:hin Limit Sell operaation kauppapaikassa halutulla markkinalla tekevän objektin
        /// </summary>
        private Button limitSellButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan ActionCentreUI:hin Check If Filled operaation kauppapaikassa halutulla markkinalla tekevän objektin
        /// </summary>
        private Button testIfFilledButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan ActionCentreUI:hin MOWWM (MoveOrdersWithWrongMarker) operaatio meidän omassa ohjelmassa halutulla markkinalla tekevän objektin
        /// </summary>
        private Button testIfRemovedFromListButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan ActionCentreUI:hin blokki, jolla voidaan resetoida parametrit käyttämällä resetointiin suunniteltuja etukäteen luotuja resetointifunktioita
        /// </summary>
        private Button resetBlocksButton;

        /// <summary>
        /// Napin referenssi, jolla luodaan ActionCentreUI:hin blokki, joka lopettaa ActionCentre blokissa koodin ajamisen sillä erää siihen ja yrittää ajaa koodia vasta seuraavalla kieroksella uudestaan.
        /// Esimerkiksi jos TestIfRemovedFromList palauttaa arvon false, voidaan koodin suoritus lopettaa siihen ja tällöin blokki jää edelliseen vaiheeseen edelleen aktiiviseksi, mutta ei resetoi mitään jne.
        /// </summary>
        private Button endForNowButton;

        /// <summary>
        /// Napin referenssi, jolla tallennetaan luodun blokkirakenteen tiedot kovalevylle
        /// </summary> 
        private Button saveComponentButton;

        /// <summary>
        /// Napin referenssi, jolla ladataan luodun blokkirakenteen tiedot kovalevyltä
        /// </summary> 
        private Button loadComponentButton;        

        /// <summary>
        /// Napin referenssi, jolla luodaan tehdystä relaatiorakennelmasta ohjeet ja tietorakenne, jota järjestelmä seuraa koodin ajon aikana 
        /// </summary>
        private Button createInstructionsButton;

        /// <summary>
        /// Koko komponentin UID numero. Koko komponentti sisältää yleensä useita blokkeja sekä niitä yhdistäviä Connection yhdysobjekteja
        /// </summary> 
        public TextBox wholeComponentUID;

        /// <summary>
        /// Koko komponentin nimi. Koko komponentti sisältää yleensä useita blokkeja sekä niitä yhdistäviä Connection yhdysobjekteja
        /// </summary> 
        public TextBox wholeComponentName;  

        /// <summary> Referenssi ObjectIndexer luokkaan, joka jakaa uniqrefnumit (eli UID) tiedot jokaiselle ohjelmaan luotavalle pysyvälle objektille </summary>
        private ObjectIndexer objindexerref;

        /// <summary> ConnectionHandler luokka ylläpitää listaa kaikista Connection objekteista </summary> 
        private ConnectionsHandler connhandler;
        
        /// <summary>
        /// Tämän hetkinen aktiivinen smartbot, jota kautta päästään slotlistarrayseen käsiksi
        /// </summary>
        private SmartBot currentsmartbot;

        /// <summary>
        /// Referenssi tiedostojen käsittelyluokkaan, jotta voidaan ladata myös tiedostoja tarvittaessa
        /// </summary>
        private FileHandler filehandle;

        /// <summary>
        /// Tämä muuttuja lisää itseään aina, kun on tehty oikeansuuntainen initialisointi luokalle. Käyttämällä propertyä IsClassInitialized saa selville, onko luokka initialisoitu oikein toimiakseen.
        /// </summary>
        private int isclassOkay=-1;

        /// <summary>
        /// Tämä property kertoo, jos luokka on initialisoitu oikein ja se voi ylipäätään toimia oikein
        /// </summary>
        public bool IsClassInitialized {
            get { 
                if (isclassOkay>0) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        /// <summary> Tämän luokan oma UID arvo </summary> 
        private long actioncentreUIownUID=-1;

        /// <summary> Tämän luokan oma UID arvo </summary> 
        public long OwnUID {
            get { return this.actioncentreUIownUID; }
            set { this.actioncentreUIownUID=value; }
        }

        /// <summary>
        /// Jos kyse on latauksesta, niin asetetaan latauksen yhteydessä arvo Loaded muuttujaan, jolla estetään ristiriitojen muodostuminen UID numeroinneissa
        /// </summary>
        public long LoadedThisInstanceOwnUID { get; set; }

        /// <summary>
        /// Tämä luokan instanssi hoitaa parametrien lukemisen ja tallentamisen useisiin luokkiin liittyen yhdellä vakioidulla tavalla
        /// </summary>
        private ParamPrinter parametriprintteri;

        /// <summary>
        /// Tiedostopääte, jota käytetään tietyntyyppisten tiedostojen tallennuksessa ohjelmassamme
        /// </summary> 
        private string file_ext;

        /// <summary>
        /// Apulista, jolla ladataan JSON objekteja tekstitiedostoista
        /// </summary>
        private List<string> loadingstrlist;

        /// <summary>
        /// Alatason parsejson tietojen purkamista varten
        /// </summary>
        public ParseJSON sublevelparsejson;

        /// <summary>
        /// Tämä instanssi pitää sisällään tarvittavat tiedot ParseJSON objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja
        /// </summary>
        public JsonParsingStruct parsestruct;

        /// <summary>
        /// Tämän objektin tason JSON objekti purettuna key value pareihin
        /// </summary>
        private SortedDictionary<string, string> thislevelJSONpairs;

        /// <summary>
        /// listofMotherBoxes lista purettuna sorteddictionaryyn
        /// </summary>
        public SortedDictionary<string, string> sublevelJSONpairs;

        /// <summary>
        /// Tämä Canvas pitää sisällään graafiset komponentit, jotka piirretään ruudulle ActionCentre:n osalta
        /// </summary>
        public Canvas canvasinuse;

        /// <summary>
        /// Tämä Dictionary pitää sisällään kaikkien MainBox komponenttien luomiseen tarvittavat parametrit
        /// </summary>
        private Dictionary<int, RectangleData> rectangleDataDict;

        /// <summary>
        /// Asema, johon päälaatikkoa asetetaan lähtökohtaisesti luodessa sivusuunnassa
        /// </summary>
        private double SetMainBoxLeft=50;
        
        /// <summary>
        /// Asema, johon päälaatikkoa asetetaan lähtökohtaisesti luodessa pystysuunnassa
        /// </summary>        
        private double SetMainBoxTop=50;
        
        /// <summary>
        /// Asema, johon infolaatikko asetetaan päälaatikon sisällä siirrettynä päälaatikon vasemmasta reunasta
        /// </summary>
        private double SetInfoOffsetLeft=10;
        
        /// <summary>
        /// Asema, johon infolaatikko asetetaan päälaatikon sisällä siirrettynä päälaatikon yläreunasta
        /// </summary>
        private double SetInfoOffsetTop=2;

        /// <summary>
        /// Instanssi, johon on kasattu stepengineen koskien yhtä blokkia tai blokkijärjestelmää, jonka perusteella ja slotin altroute tiedon perusteella järjestelmässä voidaan ajaa kohteet läpi
        /// </summary> 
        private StepEngine stepeng;

        private long stepengsuperblockuid=-1;
        /// <summary>
        /// UID StepEngineSuperBlockiin, jota käytetään parhaillaan
        /// </summary> 
        public long StepEngineSuperBlockUID {
            get { return stepengsuperblockuid; }
            private set { stepengsuperblockuid=value; }
        }

        private long stepenginstuid=-1;
        /// <summary>
        /// UID StepEngineInstruction instanssiin, jota käytetään parhaillaan
        /// TODO: Tämä tulee todennäköisesti muuttumaan jatkossa yksittäisestä luvusta johonkin muunkaltaiseen rakenteeseen
        /// </summary>
        public long StepEngineInstructionUID {
            get { return stepenginstuid; }
            private set { stepenginstuid=value; }
        }

        /// <summary>
        /// Äitiobjektin UID, jolla saadaan tietoomme minkä objektin alle tämä ActionCentreUI instanssi on luotu
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Äitiobjektin vanhemman UID 
        /// </summary> 
        public long GranParentUID { get; set; }

        /// <summary>
        /// Luodaanko UI komponentit samalla, kun luodaan toimintarakenne
        /// </summary>
        private int createuicomps;

        /// <summary>
        /// Tämän luokkan instanssi pitää sisällään ActionCentreUI luokan alta löytyvän blokkikonstruktion referenssit, kuten listofmotherboxes ja listofallboxesreferences sekä connectionhandlerin referenssin. 
        /// Näitä hyödyntämällä päästään käsiksi blokkeihin, joita voidaan ajaa yksi kerrallaan, jotta ohjelma saadaan tekemään haluttuja asioita.
        /// </summary>
        private ActionCentreConstructionHandler actcenconhan;
        
        /// <summary>
        /// Tämän luokkan instanssi pitää sisällään ActionCentreUI luokan alta löytyvän blokkikonstruktion referenssit, kuten listofmotherboxes ja listofallboxesreferences sekä connectionhandlerin referenssin. 
        /// Näitä hyödyntämällä päästään käsiksi blokkeihin, joita voidaan ajaa yksi kerrallaan, jotta ohjelma saadaan tekemään haluttuja asioita.
        /// </summary>
        public ActionCentreConstructionHandler ReturnAcUIConstructionHandler {
            get {
                return this.actcenconhan;
            }
        }

        /// <summary>
        /// Actioncentre luokan referenssi, jonka kautta saadaan rakennettua yhteys toiminnallisiin blokkeihin, joilla saadaan ajaettua itse 
        /// </summary> 
        private ActionCentre actioncent;
        /// <summary>
        /// Actioncentre luokan referenssi, jonka kautta saadaan rakennettua yhteys toiminnallisiin blokkeihin, joilla saadaan ajaettua itse 
        /// </summary>
        public ActionCentre ReturnRegisteredActionCentreRef {
            get {
                if (this.actioncent==null) {
                    this.proghmi.sendError("(ACUI)ReturnRegisteredActionCentreRef","Asked reference was null!",-1097,4,4);
                }
                return this.actioncent; 
            }
        }

        /// <summary> Constructor luokalle, joka on parametristen toimintapolkujen luontiin suunniteltu käyttöliittymä luokka </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="progstrtr"> ProgramStarter, Luokka, jonka kautta käynnistetään erilliseen threadiin kaupankäyntiä suorittava ohjelmalooppi </param>
        /// <param name="parentuid"> long, äitiobjektin UID </param>
        /// <param name="granparentuid"> long, äitiobjektin vanhemman UID </param>
        /// <returns> {void} </returns>
        public ActionCentreUI (string kutsuja, ProgramStarter progstrtr, long parentuid, long granparentuid)
        {
            string functionname="->(ACUI)Constructor";
            this.progstarter=progstrtr;
            this.objindexerref=this.progstarter.returnObjectIndexerReference;
            this.proghmi=this.progstarter.returnProgramHMIReference;
            this.file_ext=this.progstarter.ReturnMainCode.importantprogramparams.FileExt;
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.OwnUID = this.objindexerref.AddObjectToIndexer(kutsuja+functionname,this.ParentUID,(int)ObjectIndexer.indexerObjectTypes.ACTIONCENTREUI_CLASS_INSTANCE_300,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.GranParentUID); // Tässä jälkimmäinen -1 on objectindexarray

            // TODO: Luodaanko UI komponentit vai ei kohteille?! Tämä ei tule olemaan näin yksinkertainen kysymys ja tätä varmaan joutuu muuttamaan vielä koodin kehityksen myötä!
            this.createuicomps=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1;
            this.connhandler = new ConnectionsHandler(kutsuja+functionname,this.OwnUID, this.ParentUID,this.proghmi,this.objindexerref,this.file_ext,this.createuicomps);
            this.actcenconhan = new ActionCentreConstructionHandler(kutsuja+functionname,this.OwnUID,this.ParentUID,this.proghmi,this.objindexerref,this.connhandler); 

            this.loadingstrlist = new List<string>();
            this.parsestruct = new JsonParsingStruct(kutsuja+functionname,this.proghmi,true,4);
            this.sublevelparsejson = new ParseJSON(this.proghmi,true,4);
            this.canvasinuse=new Canvas { Background = Brushes.White };
            //this.currentsmartbot=this.bpact.botti[0];
            //this.currentsmartbot=this.bpact.botti[this.proghmi.TabHeaderActive];
            if (this.OwnUID>=0) {
                this.stepeng = new StepEngine(kutsuja+functionname, this.OwnUID, this.proghmi, this.objindexerref, this);
                int respo = this.objindexerref.SetObjectToIndexerWithErrorReport(kutsuja + functionname, this.OwnUID, this, (int)ObjectIndexer.rewriteOldObjectReference.ALWAYS_REWRITE_OBJECT_REFERENCE_0);
                if (respo >= 0) {
                    this.isclassOkay++;
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Failed to set object to indexer with error report. Response: " + respo, -1267, 4, 4);
                }                    
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+this.OwnUID+" ParentUid:"+this.ParentUID,-1076,4,4);
                this.isclassOkay=-5;
            }

            OperationalBlocksFactory.CreateRectangleDataDictionary(kutsuja+functionname,out this.rectangleDataDict); // Staattinen metodi listauksen luomiseksi, jossa on kaikki tarvittavat parametrit kohteiden luomiseksi
        }

        /// <summary>
        /// Tämä metodi rekisteröi BPActions luokan referenssin osaksi tämän luokan tietoja. BPActions luokan kautta päästään käsiksi esim. Smartbot luokkaan ja voidaan kysyä Slotlistojen tietoja
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="bpactio"> BPActions luokan referenssi, joka rekisteröidään osaksi ActionCentreUI luokkaa </param>
        /// <returns> {void} </returns>
        public void RegisterButtonPressedActions(string kutsuja, BPActions bpactio)
        {
            this.bpact=bpactio;
            this.isclassOkay++;
            this.currentsmartbot=this.bpact.botti[0];
            this.parametriprintteri=this.currentsmartbot.ReturnParamPrinterRef; // Otetaan Parametriprintteri instanssin referenssi talteen
            this.filehandle=this.bpact.ReturnFileHandlerRef;
            this.connhandler.InitConnectionsHandler(kutsuja,this.currentsmartbot.PriorityFileSave,this.currentsmartbot.ReturnParamPrinterRef); // Asetetaan ConnectionsHandlerille priorityfilesaver:in, jonka pitäisi löytyä currentsmartbotin alta, kuten myös ParamPrinter.
            this.actioncent = this.bpact.ReturnActionCentreRef; // Rekisteröi ActionCentre luokan ActionCentreUI luokalle toiminnasta vastaavien askelblokkien hyödyntämiseksi ja ajamiseksi
            this.actcenconhan.InitializeActionCentreConstructionHandler(kutsuja,this.actioncent); // Initialisoidaan ActionCentreConstructionHandler lopullisesti
        }

        /// <summary>
        /// Lataa annettujen tietojen perusteella halutusta kansiosta connections JSON objektin tiedostosta ja sen jälkeen tekee sille Parse toiminnon ja yrittää ladata tiedot sen perusteella
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="loadingindexstr"> string, tiedostonimen perusosan jälkeen tuleva indeksiosa. Jos tyhjä, niin nimen perusosa on sellaisenaan täydellinen tiedostonimi </param>
        /// <param name="foldername"> string, hakemisto, josta blokkistruktuuri tiedostoa ladataan </param>
        /// <param name="loadingfilename"> string, blokkistruktuuritiedoston nimi, joka aiotaan ladata </param>/// 
        /// <returns> {void} </returns>
        public void LoadConnectionsStructure(string kutsuja, string loadingindexstr, string foldername, string loadingfilename)
        {
            string functionname="->(ACUI)LoadConnectionsSturcture";
            string loadedfile="";
            long rememparseuid=-1;
            long rememparseuidtwo=-1;
            bool succeedload;
            SortedDictionary<string,string> connectiondict;
            SortedDictionary<string,string> connprop;

            if (this.IsClassInitialized==true) {

                this.loadingstrlist.Clear();

                if (loadingindexstr=="") {
                    succeedload=this.filehandle.oneLineLoading(kutsuja+functionname,loadingindexstr,foldername,loadingfilename,this.loadingstrlist,false);            
                } else {
                    succeedload=this.filehandle.oneLineLoading(kutsuja+functionname,loadingindexstr,foldername,loadingfilename,this.loadingstrlist); 
                }

                if (succeedload==true) {
                    if (loadingstrlist!=null) {
                        loadedfile=string.Join("",this.loadingstrlist);
                        if (loadedfile!="") {
                            this.parsestruct.Parserunninguid++;
                            rememparseuid=this.parsestruct.Parserunninguid;
                            connectiondict=this.parsestruct.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,this.parsestruct.Parserunninguid.ToString(),loadedfile,0); // -1 tässä kertoo, tehdäänkö debuggausta vai ei
                            if (connectiondict!=null) {
                                if (connectiondict.Count()>0) {
                                    int amo=connectiondict.Count();
                                    for (int i=0; i<amo; i++) {
                                        this.parsestruct.Parserunninguid++;
                                        rememparseuidtwo=this.parsestruct.Parserunninguid;
                                        connprop=this.parsestruct.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,this.parsestruct.Parserunninguid.ToString(),connectiondict.ElementAt(i).Value,-1);
                                        this.parsestruct.Parserunninguid++;
                                        if (connprop!=null) {    
                                            if (connprop.Count()>0) {
                                                this.ReCreateConnection(kutsuja+functionname,connprop);
                                            } else {
                                                this.proghmi.sendError(kutsuja + functionname, "Connection properties dictionary was empty!", -984, 4, 4);
                                            }
                                        } else {
                                            this.proghmi.sendError(kutsuja + functionname, "Connection properties dictionary was null!", -983, 4, 4);
                                        }
                                        this.parsestruct.Parsejson.CloseParsing(kutsuja+functionname,rememparseuidtwo.ToString());
                                    }    
                                } else {
                                    this.proghmi.sendError(kutsuja + functionname, "Connection dictionary was empty!", -982, 4, 4);
                                }
                            } else {
                                this.proghmi.sendError(kutsuja + functionname, "Connection dictionary was null!", -981, 4, 4);
                            }
                            this.parsestruct.Parsejson.CloseParsing(kutsuja+functionname,rememparseuid.ToString());
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "Loaded file was empty!", -980, 4, 4);
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Loadstrlist was null!", -979, 4, 4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Loading connection file failed!", -978, 4, 4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"This class wasn't initialized correctly! Initialization progress:"+this.isclassOkay,-1093,4,4);
            }          
        }

        /// <summary>
        /// Tämä funktio tarkistaa, onko saadussa SortedDictionaryssa kaikki tarvittavat tiedot olemassa Connectionin muodostamiseksi ja jos on, lähettää tiedot eteenpäin yhteyden luomista varten
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="oneconndict"> SortedDictionary string, string - referenssi sorteddictionary objektiin, joka pitää sisällään yhden connectionin JSON tiedot</param>
        /// <param name="brushcolorasstring">string, yhteyden väri Brush objektina, joka on käännetty string muotoon ToString käskyllä </param>
        /// <returns> {void} </returns>        
        private void ReCreateConnection(string kutsuja, SortedDictionary<string,string> oneconndict)
        {
            string functionName="->(ACUI)ReCreateConnection";
            string mandparam1="Box1OwnUID";
            string mandparam2="Box2OwnUID";
            string mandparam3="ConnectionLineStroke";
            string colorstring="";

            if (this.IsClassInitialized==true) {
                if (oneconndict!=null) {
                    if (oneconndict.Count()>0) {
                        if (oneconndict.ContainsKey(mandparam1)==true) {
                            if (oneconndict.ContainsKey(mandparam2)==true) {
                                if (oneconndict.ContainsKey(mandparam3)==true) {
                                    colorstring=oneconndict[mandparam3];
                                } else {
                                    colorstring="";
                                }
                                if (long.TryParse(oneconndict[mandparam1], out long oldbox1ownuid)) {
                                    if (long.TryParse(oneconndict[mandparam2], out long oldbox2ownuid)) {
                                        this.SearchConnectionParticipants(kutsuja+functionName,oldbox1ownuid,oldbox2ownuid,colorstring);
                                    } else {
                                        this.proghmi.sendError(kutsuja + functionName, "Failed to parse! Param:"+mandparam2+" Value:" + oneconndict[mandparam2], -970, 4, 4);
                                    }                            
                                } else {
                                    this.proghmi.sendError(kutsuja + functionName, "Failed to parse! Param:"+mandparam1+" Value:" + oneconndict[mandparam1], -969, 4, 4);
                                }
                            } else {
                                this.proghmi.sendError(kutsuja + functionName, "Mandatory param2 missing:"+mandparam2, -971, 4, 4);
                            }
                        } else {
                            this.proghmi.sendError(kutsuja + functionName, "Mandatory param1 missing:"+mandparam1, -972, 4, 4);
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionName, "OneConnectionDictionary have zero entries!", -973, 4, 4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja + functionName, "OneConnectionDictionary was null!", -974, 4, 4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionName,"This class wasn't initialized correctly! Initialization progress:"+this.isclassOkay,-1094,4,4);
            } 
        }

        /// <summary>
        /// Tämä funktio etsii onko kahden laatikon välillä yhteys. Kyseisten laatikkojen vanhat tallennuksen yhteydessä tallennetut UID tunnukset annetaan tälle funktiolle
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="box1olduid"> long, vanha tallennuksen yhteydessä talletettu lähtölaatikon UID, jota etsitään LoadedOwnUID propertyistä </param>
        /// <param name="box2olduid"> long, vanha tallennuksen yhteydessä talletettu tulolaatikon UID, jota etsitään LoadedOwnUID propertyistä </param>
        /// <param name="brushcolorasstring">string, yhteyden väri Brush objektina, joka on käännetty string muotoon ToString käskyllä </param>
        /// <returns> {void} </returns>
        private void SearchConnectionParticipants(string kutsuja, long box1olduid, long box2olduid, string brushcolorasstring)
        {
            string functionname="->(ACUI)SearchConnectionParticipants";
            long box1uid=-1;
            long box2uid=-1;
            long box1key=-1;
            long box2key=-1;
            int retVal=-1;

            if (this.IsClassInitialized==true) {
                if (this.actcenconhan.listofAllBoxesReferences!=null) {
                    if (this.actcenconhan.listofAllBoxesReferences.Count>0) {
                        int amo=this.actcenconhan.listofAllBoxesReferences.Count();
                        for (int i=0; i<amo; i++) {
                            if (this.actcenconhan.listofAllBoxesReferences.ElementAt(i).Value.LoadedOwnUID==box1olduid) {
                                box1uid=this.actcenconhan.listofAllBoxesReferences.ElementAt(i).Value.OwnUID;
                                box1key=this.actcenconhan.listofAllBoxesReferences.ElementAt(i).Key;
                            }
                            if (this.actcenconhan.listofAllBoxesReferences.ElementAt(i).Value.LoadedOwnUID==box2olduid) {
                                box2uid=this.actcenconhan.listofAllBoxesReferences.ElementAt(i).Value.OwnUID;
                                box2key=this.actcenconhan.listofAllBoxesReferences.ElementAt(i).Key;
                            }
                            if (box1uid>-1 && box2uid>-1) {
                                break;
                            }
                        }
                        if (box1uid>-1 && box2uid>-1) {
                            retVal=this.ConnectBoxes(kutsuja+functionname,this.canvasinuse,this.actcenconhan.listofAllBoxesReferences[box1key].RectangleObject,this.actcenconhan.listofAllBoxesReferences[box2key].RectangleObject,brushcolorasstring);
                            if (retVal<0) {
                                this.proghmi.sendError(kutsuja+functionname,"Error in creation of connection! Response:"+retVal,-1055,4,4);
                            }
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "Couldn't find both boxes! Box1UID:"+box1uid+" OldBox1UID:"+box1olduid+" Box2UID:"+box2uid+" OldBox2UID:"+box2olduid, -976, 4, 4);
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "ListofAllBoxesReferences have zero entries!", -975, 4, 4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "ListofAllBoxesReferences was null!", -977, 4, 4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"This class wasn't initialized correctly! Initialization progress:"+this.isclassOkay,-1095,4,4);
            } 
        }

        /// <summary>
        /// Lataa blokkitiedoston ja asettaa sen tiedot tähän tietostruktuuriin
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="loadingindexstr"> string, tiedostonimen perusosan jälkeen tuleva indeksiosa. Jos tyhjä, niin nimen perusosa on sellaisenaan täydellinen tiedostonimi </param>
        /// <param name="foldername"> string, hakemisto, josta blokkistruktuuri tiedostoa ladataan </param>
        /// <param name="loadingfilename"> string, blokkistruktuuritiedoston nimi, joka aiotaan ladata </param>
        /// <returns> {void} </returns>
        public void LoadBlockStructure(string kutsuja, string loadingindexstr, string foldername, string loadingfilename)
        {
            string functionname="->(ACUI)LoadBlockStructure";
            string loadedfile="";
            long rememparseuid=-1;
            bool succeedload;

            if (this.IsClassInitialized==true) {

                this.loadingstrlist.Clear();

                if (loadingindexstr=="") {
                    succeedload=this.filehandle.oneLineLoading(kutsuja+functionname,loadingindexstr,foldername,loadingfilename,this.loadingstrlist,false);
                } else {
                    succeedload=this.filehandle.oneLineLoading(kutsuja+functionname,loadingindexstr,foldername,loadingfilename,this.loadingstrlist);
                }

                if (succeedload==true) {
                    if (loadingstrlist!=null) {
                        loadedfile=string.Join("",this.loadingstrlist);
                        if (loadedfile!="") {
                            this.parsestruct.Parserunninguid++;
                            rememparseuid=this.parsestruct.Parserunninguid;
                            this.thislevelJSONpairs=this.parsestruct.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,this.parsestruct.Parserunninguid.ToString(),loadedfile,0);
                            if (this.thislevelJSONpairs!=null) {
                                if (this.thislevelJSONpairs.Count>0) {
                                    this.parametriprintteri.SetActionCentreUIObjectToPrint(this);
                                    this.parametriprintteri.SetActionCentreUIParamValues(kutsuja+functionname,this.thislevelJSONpairs,(int)ParamPrinter.JsonObjectLoadingOptions.REGULAR_LOADING_1);
                                    int amo=this.thislevelJSONpairs.Count;
                                    for (int i=0; i<amo; i++) {
                                        this.proghmi.sendError(kutsuja+functionname,"Key:"+this.thislevelJSONpairs.ElementAt(i).Key+" Value:"+this.thislevelJSONpairs.ElementAt(i).Value,903,2,3);
                                    }
                                } else {
                                    this.proghmi.sendError(kutsuja+functionname,"Error during deserialization of JSON! JSONPairs Count was zero! File:"+loadedfile,-904,4,4);
                                }
                            } else {
                                this.proghmi.sendError(kutsuja+functionname,"Error during deserialization of JSON! JSONPairs was null! File:"+loadedfile,-903,4,4);
                            }
                            this.parsestruct.Parsejson.CloseParsing(kutsuja+functionname,rememparseuid.ToString()); // Suljetaan parsing
                        } else {
                            this.proghmi.sendError(kutsuja+functionname,"Returned file was empty! ",-902,4,4);
                        }
                    } else {
                        this.proghmi.sendError(kutsuja+functionname,"Returned list was null! ",-901,4,4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"File loading failed! Folder:"+ImportantProgramParams.ActionsSavingFolderName+" File:"+ImportantProgramParams.ActionsBlockBasicName+"_"+this.wholeComponentName,-900,2,4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"This class wasn't initialized correctly! Initialization progress:"+this.isclassOkay,-1096,4,4);
            } 
        }

        /// <summary>
        /// Palauttaa ActionCentreUI objektin ja sen kaikkien olennaisten aliobjektien tiedot JSON tyyppisenä string merkkijonona tallennusta varten
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {string} Palauttaa ActionCentreUI objektin ja sen kaikkien olennaisten aliobjektien tiedot JSON tyyppisenä string merkkijonona tallennusta varten </returns>
        public string ReturnThisObjectParametersAsJSONForSaving(string kutsuja)
        {
            string functionname="->(ACUI)ReturnThisObjectParametersAsJSONForSaving";
            string retVal="";

            retVal=this.ReturnThisObjectAndSubobjsParametersAsJSON(kutsuja+functionname,ParamNameLists.actioncentreUISavingParameterNames);

            return retVal;
        }

        /// <summary>
        /// Palauttaa ActionCentreUI objektin ja sen kaikkien olennaisten aliobjektien tiedot JSON tyyppisenä string merkkijonona tallennusta tai printtausta varten
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="actioncentreuiownparams"> List string, referenssi parametri listaan jonka tiedot halutaan palautettavan JSON objekti muodossa </param>
        /// <returns> {string} Palauttaa ActionCentreUI objektin ja sen kaikkien olennaisten aliobjektien tiedot JSON tyyppisenä string merkkijonona tallennusta tai printtausta varten </returns>
        public string ReturnThisObjectAndSubobjsParametersAsJSON(string kutsuja, List<string> actioncentreuiownparams)
        {
            string functionname="->(ACUI)ReturnThisObjectAndSubobjsParametersAsJSON";
            string retVal="";
            string intendthis="";
            if (this.IsClassInitialized==true) {
                if (actioncentreuiownparams!=null) {
                    if (actioncentreuiownparams.Count>0) {
                        this.parametriprintteri.SetActionCentreUIObjectToPrint(this);
                        intendthis= this.parametriprintteri.MyOwnParamPrint(kutsuja+functionname,actioncentreuiownparams,(int)ParamPrinter.myOwnTypePrintingEnum.JSON_OBJECT_WITH_PARAM_NAMES_AND_VALUES_2);
                        retVal= ParseJSON.IntendSerializedJSON(kutsuja+functionname,intendthis);
                    } else {
                        this.proghmi.sendError(kutsuja+functionname,"Parameter list didn't contain any parameter name (JSONparameters)!",-898,4,4);
                        retVal="ERROR=-110";
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Parameter list was null (JSONparameters)!",-899,4,4);
                    retVal="ERROR=-111";
                }                 
            } else {
                retVal="";
                this.proghmi.sendError(kutsuja+functionname,"This class wasn't initialized correctly! Initialization progress:"+this.isclassOkay,-897,4,4);
            }
            return retVal;
        }

        /// <summary> Luo pääsuorakulmion sisään pienempiä suorakulmioita sen reunoihin, jotka voi yhdistää toisiinsa. Tällä hetkellä värivaihtoehtoja on vihreä = sisään tuleva arvo, keltainen = vertailuarvo ja punainen = vertailun lopputulos </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="color"> Brush, väri jolla laatikko luodaan </param>
        /// <param name="width"> double, luotavan ja toisiinsa viivalla yhdistettävän pikkulaatikon leveys </param>
        /// <param name="height"> double, luotavan ja toisiinsa viivalla yhdistettävän pikkulaatikon korkeus </param>
        /// <param name="objectOwnUID"> long, luotavan laatikon oma uniqrefnum, jolla siihen liitetty tietomalli on rekisteröity ja se on löydettävissä esim. ObjectIndexerin objectlist:stä. </param>
        /// <returns> {Rectangle} palauttaa luodun laatikon referenssin </returns>
        private Rectangle CreateSubBox(string kutsuja, Canvas canvas, Brush color, double width, double height, long objectOwnUID)
        {
            string functionname="->(ACUI)CreateSubBox";
            Rectangle box = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = color,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Tag = objectOwnUID
            };

            box.MouseDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (isConnecting)
                    {
                        if (firstSelectedBox == null)
                        {
                            firstSelectedBox = box;
                            connectionStatusText.Text = "Paina 2. laatikkoa";
                        }
                        else if (secondSelectedBox == null && firstSelectedBox != box)
                        {
                            secondSelectedBox = box;
                            int connectsucc=ConnectBoxes(kutsuja+functionname,canvas, firstSelectedBox, secondSelectedBox);
                            if (connectsucc<0) {
                                this.proghmi.sendError(kutsuja+functionname,"Error during connecting 2 boxes! Response:"+connectsucc,-1056,4,4);
                            }
                            firstSelectedBox = null;
                            secondSelectedBox = null;
                            isConnecting = false;
                            connectButton.Content = "Lisää yhteys";
                            connectionStatusText.Text = "";

                            this.SetDisableEnableButtons(kutsuja+functionname,isConnecting,connectButton);
                        }
                        else
                        {
                            firstSelectedBox = null;
                            secondSelectedBox = null;
                            isConnecting = false;
                            connectButton.Content = "Lisää yhteys";
                            connectionStatusText.Text = "";

                            this.SetDisableEnableButtons(kutsuja+functionname,isConnecting,connectButton);                           
                        }
                    }
                    else if (isDisconnecting)
                    {
                        if (firstSelectedBox == null)
                        {
                            firstSelectedBox = box;
                            connectionStatusText.Text = "Paina 2. laatikkoa";
                        }
                        else if (secondSelectedBox == null && firstSelectedBox != box)
                        {
                            secondSelectedBox = box;
                            var connection = this.connhandler.connections.FirstOrDefault(c => (c.Value.ReturnConnectionUIComponents.Box1 == firstSelectedBox && c.Value.ReturnConnectionUIComponents.Box2 == secondSelectedBox) || (c.Value.ReturnConnectionUIComponents.Box2 == firstSelectedBox && c.Value.ReturnConnectionUIComponents.Box1 == secondSelectedBox));
                            if (connection.Value != null)
                            {
                                DisconnectBoxes(kutsuja+functionname,connection.Value,canvas);
                                connectionStatusText.Text = "Yhteys poistettu";
                            }
                            else
                            {
                                connectionStatusText.Text = "Ei yhteyttä komponenttien välillä";
                            }
                            firstSelectedBox = null;
                            secondSelectedBox = null;
                            isDisconnecting = false;
                            disconnectButton.Content = "Poista yhteys";

                            connectButton.IsEnabled=!isDisconnecting;
                            deleteButton.IsEnabled=!isDisconnecting;
                        }
                    }
                }
            };

            return box;
        }

        /// <summary>
        /// Tämä funktio sijoittaa valitun operaattorin arvon itse tietoja säilyttävän luokan vastaavalle kohteelle
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        /// <returns> {void} </returns>
        private void OperatorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string functionname="->(ACUI)OperatorComboBox_SelectionChanged";
            long comboUID=-1;
            long parentUID=-1;

            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboUID = (long)comboBox.Tag;
                if (comboUID>=0) {
                    string selectedOperator = comboBox.SelectedItem as string;
                    if (selectedOperator != null)
                    {
                        // Tee jotain valitun operaattorin kanssa
                        parentUID=this.objindexerref.objectlist[comboUID].ParentUID;
                        if (parentUID>=0) {
                            if (this.actcenconhan.listofMotherBoxes.IndexOfKey(parentUID)>-1) {
                                // Asetetaan valittu operaattori kohteelle tiedoksi
                                this.actcenconhan.listofMotherBoxes[parentUID].StoredUIcomps.SetSelectedCombo(functionname,selectedOperator,false);
                            } else {
                                this.proghmi.sendError(functionname,"No such parent in list! Missing parentUID:"+parentUID+" UID:"+comboUID,-754,4,4);
                            }
                        } else {
                            this.proghmi.sendError(functionname,"Invalid Parent! UID:"+comboUID+" ParentUID:"+parentUID,-753,4,4);
                        }
                    } else {
                        this.proghmi.sendError(functionname,"Selected operator null! UID:"+comboUID+" Selectedoperator:"+selectedOperator,-752,4,4);
                    }
                } else {
                    this.proghmi.sendError(functionname,"No valid UID in combobox! UID:"+comboUID,-751,4,4);
                }
            }
        }

        /// <summary>
        /// Tämä funktio sijoittaa valitun operaattorin arvon itse tietoja säilyttävän luokan vastaavalle kohteelle
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        /// <returns> {void} </returns>
        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string functionname="->(ACUI)GroupComboBox_SelectionChanged";
            long comboUID=-1;
            long parentUID=-1;

            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboUID = (long)comboBox.Tag;
                if (comboUID>=0) {
                    string selectedGroup= comboBox.SelectedItem as string;
                    if (selectedGroup != null)
                    {
                        // Tee jotain valitun operaattorin kanssa
                        parentUID=this.objindexerref.objectlist[comboUID].ParentUID;
                        if (parentUID>=0) {
                            if (this.actcenconhan.listofMotherBoxes.IndexOfKey(parentUID)>-1) {
                                this.actcenconhan.listofMotherBoxes[parentUID].StoredUIcomps.SetGroupCombo(functionname,selectedGroup);
                            } else {
                                this.proghmi.sendError(functionname,"No such parent in list! Missing parentUID:"+parentUID+" UID:"+comboUID,-757,4,4);
                            }
                        } else {
                            this.proghmi.sendError(functionname,"Invalid Parent! UID:"+comboUID+" ParentUID:"+parentUID,-758,4,4);
                        }
                    } else {
                        this.proghmi.sendError(functionname,"Selected operator null! UID:"+comboUID+" Selectedoperator:"+selectedGroup,-759,4,4);
                    }
                } else {
                    this.proghmi.sendError(functionname,"No valid UID in combobox! UID:"+comboUID,-760,4,4);
                }
            }
        }        

        /// <summary> Tämä funktio luo sisällön blokkeihin ja rekisteröi siihen kuuluvat listofMotherBoxes luokkaan </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="listofmotherboxesuid"> long, se uid joka on annettu pääkomponentille, jonka alle nämä tiedot rekisteröidään </param>
        /// <param name="boxtype"> int, printattavan päälaatikon tyyppi - vaikuttaa varsinkin operatorComboBoxin sisältöön </param>
        /// <param name="textcolor"> int, parametrin väri sisäisen indeksin numerolla annettuna </param>
        /// <param name="contentpane"> StackPanel, se komponentti jonka alle nämä tiedot asetetaan käyttöliittymässä </param>
        /// <returns> {void} </returns>        
        private void CreateContentBlocks(string kutsuja, long listofmotherboxesuid, int boxtype, int textcolor, StackPanel contentpane)
        {
            string functionname="->(ACUI)CreateContentBlocks";
            double textboxWidth=90;
            // Name TextBlock and TextBox
            WrapPanel wrapContent1 = new WrapPanel();
            TextBlock nameLabel = new TextBlock { Text = "Name:", Foreground=this.OwnColorList(kutsuja+functionname,textcolor) };
            TextBox nameTextBox = new TextBox { Width=textboxWidth };
            wrapContent1.Children.Add(nameLabel);
            wrapContent1.Children.Add(nameTextBox);
            contentpane.Children.Add(wrapContent1);

            // RouteId TextBlock and TextBox
            WrapPanel wrapContent2 = new WrapPanel();
            TextBlock routeIdLabel = new TextBlock { Text = "RouteId:", Foreground=this.OwnColorList(kutsuja+functionname,textcolor) };
            TextBox routeIdTextBox = new TextBox { Width=textboxWidth-10 };
            wrapContent2.Children.Add(routeIdLabel);
            wrapContent2.Children.Add(routeIdTextBox);
            contentpane.Children.Add(wrapContent2);

            
            // ComboBox for operator selection
            TextBlock groupLabel = new TextBlock { Text = "Group:", Foreground=this.OwnColorList(kutsuja+functionname,textcolor) };
            ComboBox groupComboBox = new ComboBox();
            this.AddGroupItems(kutsuja+functionname,boxtype,groupComboBox); // Lisätään listaan operaatiotyypit riippuen boxtypestä

            // Lisää tapahtumankäsittelijä, joka laittaa aina operaattorin talteen itse luokalle myös
            groupComboBox.SelectionChanged += GroupComboBox_SelectionChanged;     

            // HUOM HUOM - Kohteet luodaan aina, mutta riippuen kohteen tyypistä, riippuu kiinnitetäänkö ne infopaneliin, joka tuo ne näkyviin
            if (boxtype==(int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100) {
                contentpane.Children.Add(groupLabel);
                contentpane.Children.Add(groupComboBox);
            }      

            // ComboBox for operator selection
            TextBlock operatorLabel = new TextBlock { Text = "Operator:", Foreground=this.OwnColorList(kutsuja+functionname,textcolor) };
            ComboBox operatorComboBox = new ComboBox();
            this.AddOperatorItems(kutsuja+functionname,boxtype,operatorComboBox); // Lisätään listaan operaatiotyypit riippuen boxtypestä

            // Lisää tapahtumankäsittelijä, joka laittaa aina operaattorin talteen itse luokalle myös
            operatorComboBox.SelectionChanged += OperatorComboBox_SelectionChanged;

            // ... Add more operators as needed
            contentpane.Children.Add(operatorLabel);
            contentpane.Children.Add(operatorComboBox);

            // 2 TextBlocks for additional values
            int i=1;
            TextBlock valueTextBlock1 = new TextBlock { Text = "Value " + (i + 1), Foreground=this.OwnColorList(kutsuja+functionname,textcolor) };
            if (boxtype!=(int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100) { // HUOM HUOM - Kohteet luodaan aina, mutta riippuen kohteen tyypistä, riippuu kiinnitetäänkö ne infopaneliin, joka tuo ne näkyviin
                contentpane.Children.Add(valueTextBlock1);
            }
            i++;
            TextBlock valueTextBlock2 = new TextBlock { Text = "Value " + (i + 1), Foreground=this.OwnColorList(kutsuja+functionname,textcolor) };
            if (boxtype!=(int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100) { // HUOM HUOM - Kohteet luodaan aina, mutta riippuen kohteen tyypistä, riippuu kiinnitetäänkö ne infopaneliin, joka tuo ne näkyviin
                contentpane.Children.Add(valueTextBlock2);
            }
            
            // Rekisteröidään luodut komponentit
            this.actcenconhan.listofMotherBoxes[listofmotherboxesuid].RegisterUIComponents(kutsuja,nameTextBox,routeIdTextBox,groupComboBox, operatorComboBox,valueTextBlock1,valueTextBlock2);
        }

        /// <summary> Tämä funktio asettaa referenssin comboboxiin sen sisältöä vastaavat OperatorItem tiedot </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="boxtype"> int, printattavan päälaatikon tyyppi - vaikuttaa varsinkin operatorComboBoxin sisältöön </param>
        /// <param name="comboboxref"> Combobox, comboboxin referenssi johon comboboxin itemit halutaan sisällytettävän </param>
        /// <returns> {void} </returns>  
        private void AddOperatorItems(string kutsuja, int boxtype, ComboBox comboboxref)
        {
            string functionname="->(ACUI)AddOperatorItems";
            if (comboboxref!=null) {
                switch (boxtype)
                {
                    case (int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_NORMAL_IF_1:
                        comboboxref.Items.Add("==");
                        comboboxref.Items.Add("!=");
                        comboboxref.Items.Add(">");
                        comboboxref.Items.Add(">=");
                        comboboxref.Items.Add("<");
                        comboboxref.Items.Add("<=");
                        break;
                    case (int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_BETWEEN_OUTSIDE_2:
                        comboboxref.Items.Add("> between <");
                        comboboxref.Items.Add(">= between <");
                        comboboxref.Items.Add("> between <=");
                        comboboxref.Items.Add(">= between <=");
                        comboboxref.Items.Add("outside > < outside");
                        comboboxref.Items.Add("outside >= < outside");
                        comboboxref.Items.Add("outside > <= outside");
                        comboboxref.Items.Add("outside >= <= outside");
                        break;
                    case (int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_200:
                        comboboxref.Items.Add("+");
                        comboboxref.Items.Add("-");
                        comboboxref.Items.Add("/");
                        comboboxref.Items.Add("*");
                        break;                                        
                    default:
                        comboboxref.Items.Add("None");
                        this.proghmi.sendError(kutsuja+functionname,"No such operator item type! Boxtype:"+boxtype,-738,4,4);
                        break;
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Combobox reference was null! Boxtype:"+boxtype,-739,4,4);
            }
        }

        /// <summary> Tämä funktio asettaa referenssin comboboxiin sen sisältöä vastaavat GropupItem tiedot </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="boxtype"> int, printattavan päälaatikon tyyppi - vaikuttaa varsinkin operatorComboBoxin sisältöön </param>
        /// <param name="comboboxref"> Combobox, comboboxin referenssi johon comboboxin itemit halutaan sisällytettävän </param>
        /// <returns> {void} </returns> 
        private void AddGroupItems(string kutsuja, int boxtype, ComboBox comboboxref)
        {
            string functionname="->(ACUI)AddGroupItems";
            if (comboboxref!=null) {
                switch (boxtype)
                {
                    case (int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100:
                        comboboxref.Items.Add("SLOT_VALUES");
                        comboboxref.Items.Add("COURSE_INFO");
                        comboboxref.Items.Add("MAIN_PARAMS");
                        comboboxref.Items.Add("TRIGGERLIST_PARAMS");
                        break;                                       
                    default:
                        comboboxref.Items.Add("None");
                        this.proghmi.sendError(kutsuja+functionname,"No such operator item type! Boxtype:"+boxtype,-755,4,4);
                        break;
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Combobox reference was null! Boxtype:"+boxtype,-756,4,4);
            }
        }

        /// <summary> Tämä funktio palauttaa sisäistä indeksiä vastaavan värin </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="colorindex"> int, värin sisäinen indeksi </param>
        /// <returns> {SolidColorBrush} palauttaa sisäistä indeksiä vastaavan värin </returns>        
        private SolidColorBrush OwnColorList(string kutsuja, int colorindex)
        {
            switch (colorindex)
            {
                case 0:
                    return Brushes.White;
                case 1:
                    return Brushes.Black;
                case 2:
                    return Brushes.DarkGray;
                case 3:
                    return Brushes.Magenta;
                case 4:
                    return Brushes.GreenYellow;
                case 5:
                    return Brushes.Orange;
                case 6:
                    return Brushes.Red;
                case 7:
                    return Brushes.Blue;
                case 8:
                    return Brushes.Green;
                case 9:
                    return Brushes.Yellow;
                case 10:
                    return Brushes.Brown;
                case 11:
                    return Brushes.Navy;
                case 12:
                    return Brushes.Cyan;
                case 13:
                    return Brushes.Coral;
                case 14:
                    return Brushes.Violet;
                case 15:
                    return Brushes.Beige;
                case 16:
                    return Brushes.DarkSeaGreen;
                default:
                    return Brushes.Coral;
            }
        }

        /// <summary> Kutsu tätä metodia, kun haluat lisätä värit ComboBoxiin (esimerkiksi Window_Loaded-tapahtumassa) </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>        
        /// <returns> {void} </returns>
        private void AddColorsToComboBox(string kutsuja)
        {
            string functionname="->(ACUI)AddColorsToComboBox";
            for (int i=1; i<13; i++) {
                this.colorComboBox.Items.Add(this.OwnColorList(kutsuja+functionname,i));
            }
        }

        /// <summary> Tapahtumankäsittelijä, joka suoritetaan, kun käyttäjä valitsee värin colorComboBoxista </summary>
        /// <returns> {void} </returns>
        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ota valittu väri talteen
            selectedColor = (Brush)colorComboBox.SelectedItem;
        }

        /// <summary>
        /// Tämä funktio tallentaa komponenttien tiedot kovalevylle asynkroonisti käyttäen PrioritySaver luokkaa
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>        
        /// <returns> {void} </returns>
        private void SaveComponentInformation(string kutsuja)
        {
            string functionname="->(ACUI)SaveConponentInformation";
            this.connhandler.SaveAllConnections(kutsuja+functionname,this.wholeComponentName.Text);
            this.SaveAllBlocks(kutsuja+functionname);
        }

        /// <summary>
        /// Overload #0 - Tämä funktio lataa komponenttien tiedot kovalevyltä SYNKROONISTI, mutta tätä funktiota voi pääsääntöisesti käyttää vain ladattaessa buttonin kautta
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>        
        /// <returns> {void} </returns>
        public void LoadComponentInformation(string kutsuja)
        {
            string functionname="->(ACUI)LoadComponentInformation#0";
            this.LoadBlockStructure(kutsuja+functionname, this.wholeComponentName.Text,ImportantProgramParams.ActionsSavingFolderName,ImportantProgramParams.ActionsBlockBasicName);
            this.LoadConnectionsStructure(kutsuja+functionname,this.wholeComponentName.Text,ConnectionsHandler.ConnectionsSavingFolderName,ConnectionsHandler.ConnectionsBlockBasicName);
        }

        /// <summary>
        /// Overload #1 - Tämä funktio lataa komponenttien tiedot kovalevyltä SYNKROONISTI, mutta tätä funktiota voi automaattisesti parametrien latauksen kautta tietämällä ladattavan kohteen nimen
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="filenameindex"> string, tiedostojen nimen loppuosa, josta pyritään lukemaan kohteen tiedot sitä vastaavasta moduuliblokin JSON tiedostosta </param>
        /// <returns> {void} </returns>
        public void LoadComponentInformation(string kutsuja, string filenameindex)
        {
            string functionname="->(ACUI)LoadComponentInformation#1";
            this.LoadBlockStructure(kutsuja+functionname, filenameindex,ImportantProgramParams.ActionsSavingFolderName,ImportantProgramParams.ActionsBlockBasicName);
            this.LoadConnectionsStructure(kutsuja+functionname,filenameindex,ConnectionsHandler.ConnectionsSavingFolderName,ConnectionsHandler.ConnectionsBlockBasicName);            
        }

        /// <summary>
        /// Tämä metodi tallentaa kaikki connectionit ja niiden tiedot. Tämä metodin kutsuminen ei kuitenkaan ole aina järkevää, koska connection tiedot kannattaa toisinaan koostaa osaksi suurempaa tiedostoa, jota varten kannattaa käyttää PrintAllConnections käskyä
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {int} Palauttaa 1, jos tietojen lähettäminen tallennukseen onnistui. Jos palauttaa pienempi kuin 0, niin kyseessä oli virhe! </returns>
        public int SaveAllBlocks(string kutsuja)
        {
            string functionname="->(ACUI)SaveAllBlocks";
            int retVal=-1;
            string savestring="";

            if (this.IsClassInitialized==true) {
                savestring=this.ReturnThisObjectParametersAsJSONForSaving(kutsuja+functionname);
                if (savestring!="") {
                    this.currentsmartbot.PriorityFileSave.AddFileSaveItem(kutsuja+functionname,ImportantProgramParams.ActionsSavingFolderName,ImportantProgramParams.ActionsBlockBasicName+"_"+this.wholeComponentName.Text+this.file_ext,savestring,2000001,true);
                    retVal=1;
                } else {
                    retVal=-3;
                    this.proghmi.sendError(kutsuja+functionname,"Saving string was empty!",-878,4,4);
                }
            } else {
                retVal=-2;
                this.proghmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassOkay,-879,4,4);
            }
            return retVal;  
        }        

        /// <summary>
        /// Tarkistaa, että annetussa merkkijonossa on tarpeeksi kirjaimia. Jos ei ole, 
        /// funktio täydentää merkkijonoa kysymysmerkeillä ja lähettää virheilmoituksen.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku (virheilmoitusta varten).</param>
        /// <param name="letters">Referenssi merkkijonoon, joka sisältää laatikoihin tulostettavat kirjaimet.</param>
        /// <param name="boxCount">Kuinka monta laatikkoa on luotavana.</param>
        /// <param name="boxColor">Laatikoiden väri (käytetään virheilmoituksessa).</param>
        /// <param name="functionname">Funktion nimi, josta tämä apufunktio on kutsuttu (virheilmoitusta varten).</param>
        /// <returns> {void} </returns>
        private void CheckAndFillLetters(string kutsuja, ref string letters, int boxCount, string boxColor)
        {
            string functionname="->(ACUI)CheckAndFillLetters";
            if (boxCount > letters.Length)
            {
                int deficit = boxCount - letters.Length;
                this.proghmi.sendError(kutsuja + functionname, $"Kirjainmäärä ei vastaa {boxColor} laatikoiden määrää! Puuttuu {deficit} kirjainta.", -767, 4, 4);
                letters = letters.PadRight(boxCount, '?');
            }
        }

        /// <summary>
        /// Tämä metodi luo AddRectangle laatikon ladatun JSON objektin perusteella
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="jsonstringkey">string, json objektin key, jonka perusteella päälaatikkoa lähdetään lataamaan. </param>
        /// <param name="jsonstringvalue">string, json objektin value, jonka perusteella päälaatikkoa lähdetään lataamaan. </param>
        /// <param name="isloaded"> int, jos 0, niin ei olle tietojen latauksesta kyse ja jos 1, niin UID tiedot menevät erillisiin Loaded muuttujiin varsinaisten muuttujien sijasta </param>
        /// <returns> {MotherConnectionRectangle} palauttaa objektin, joka on luotu jsonobjektin perusteella päälaatikolle. Kyseinen objekti sisältää myös kohteen graafisen komponentin. Palauttaa null ja antaa virheilmoituksen, jos jsonobjektin perusteella luonti ei onnistunut (esim. json objekti virheellinen tai sisältää vääriä tietoja) </returns>
        public MotherConnectionRectangle LoadParameterRectangles(string kutsuja, string jsonstringkey, string jsonstringvalue, int isloaded=1)
        {
            string functionname="->(ACUI)LoadParameterRectangles";
            long rememparseuid=-1;
            long rememparseuidtwo=-1;
            string mandatoryparam1="BlockType";
            string mandatoryparam2="mainboxConnectionRect";
            string mandatoryparam3="RectangleObjectTop";
            string mandatoryparam4="RectangleObjectLeft";
            int canset=0;

            MotherConnectionRectangle retObj=null;
            
            if (jsonstringvalue!="") {
                SortedDictionary<string, string> sorteddictref;
                SortedDictionary<string, string> mainboxdictref;
                this.parsestruct.Parserunninguid++;
                rememparseuid=this.parsestruct.Parserunninguid;
                sorteddictref=this.parsestruct.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,this.parsestruct.Parserunninguid.ToString(),jsonstringvalue,-1); // Tässä -1 tarkoittaa, ettei tehdä debug printtausta
                this.parsestruct.Parserunninguid++;

                if (sorteddictref!=null) {
                    if (sorteddictref.ContainsKey(mandatoryparam1)==true && sorteddictref.ContainsKey(mandatoryparam2)==true) {

                        rememparseuidtwo=this.parsestruct.Parserunninguid;
                        mainboxdictref=this.parsestruct.Parsejson.DeserializeOneLevelFromJSON(kutsuja+functionname,this.parsestruct.Parserunninguid.ToString(),sorteddictref[mandatoryparam2],-1); // Tässä -1 tarkoittaa, ettei tehdä debug printtausta
                        this.parsestruct.Parserunninguid++;

                        if (mainboxdictref!=null) {

                            if (mainboxdictref.ContainsKey(mandatoryparam3)==true && mainboxdictref.ContainsKey(mandatoryparam4)==true) {

                                if (double.TryParse(mainboxdictref[mandatoryparam3], out double ownsettop)) {
                                    canset++;
                                } else {
                                    this.proghmi.sendError(kutsuja+functionname,"Problems to parse ownsettop!",-936,4,4); 
                                }
                                if (double.TryParse(mainboxdictref[mandatoryparam4], out double ownsetleft)) {
                                    canset++;
                                } else {
                                    this.proghmi.sendError(kutsuja+functionname,"Problems to parse ownsetleft!",-935,4,4); 
                                }
                                if (int.TryParse(sorteddictref[mandatoryparam1], out int ownboxtype)) {
                                    canset++;
                                } else {
                                    this.proghmi.sendError(kutsuja + functionname, "Failed to parse Mandatory parameter! Value:" + sorteddictref[mandatoryparam1], -928, 4, 4);
                                }                                    

                                if (canset>=3) {
                                    // Luo MotherConnectionRectanglelle yksilöllisen UID arvon
                                    long uid = this.objindexerref.AddObjectToIndexer(kutsuja+functionname,this.OwnUID,(int)ObjectIndexer.indexerObjectTypes.MOTHER_COMPONENT_RECTANGLE_102,-1); // TODO: Tässä ensimmäinen -1 on parentin UID, joka pitäisi korvata jollain. Nyt -1 tarkoittaa, että tämän yläpuolella ei ole mitään, vaikka oikeasti on se templaatti, johon tämä komponentti kuuluu
                                    if (uid>=0) {
                                        // luo mainBox MotherConnectionRectangle objektin pääkomponentiksi sekä siihen liittyvän graafisen komponentin Canvas kanvasille
                                        Rectangle mainBox=this.LoadMainBox(kutsuja+functionname,uid,ownboxtype,this.canvasinuse,sorteddictref,ownsetleft,ownsettop);

                                        if (mainBox!=null) {
                                            if (this.actcenconhan.listofMotherBoxes.IndexOfKey(uid)>-1) {
                                                StackPanel contentPanel=this.LoadContentPanel(kutsuja+functionname,uid,ownboxtype,this.canvasinuse,ownsetleft,ownsettop);
                                                this.AddRectangleSubMethodsByType(kutsuja+functionname,this.canvasinuse,ownboxtype,mainBox,uid);
                                                retObj=this.actcenconhan.listofMotherBoxes[uid];

                                                retObj.LoadMainBoxConnectionRectParams(kutsuja+functionname,this.parsestruct,sorteddictref,isloaded);
                                                retObj.LoadSmallRectangleParams(kutsuja+functionname,this.parsestruct,sorteddictref,isloaded);
                                                retObj.LoadStoredUIObjectParams(kutsuja+functionname,this.parsestruct,sorteddictref,isloaded);
                                            } else {
                                                this.proghmi.sendError(kutsuja+functionname,"List of main components didn't contain correct UID!",-932,4,4);                                
                                            }
                                        } else {
                                            this.proghmi.sendError(kutsuja+functionname,"Couldn't create mainBox! Returned null!",-931,4,4);
                                        }
                                    } else {
                                        this.proghmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+uid+" ParentUid:"+this.OwnUID,-1077,4,4);
                                    }

                                } else {
                                    this.proghmi.sendError(kutsuja+functionname,"Couldn't parse all 3 parameters!",-937,4,4); 
                                }
                            } else {
                                this.proghmi.sendError(kutsuja+functionname,"Mainbox sorteddictionary missing mandatory params! Param1:"+mandatoryparam3+" Param2:"+mandatoryparam4,-934,4,4);
                            }
                        } else {
                            this.proghmi.sendError(kutsuja+functionname,"Mandatory object was null!",-933,4,4);
                        }

                        this.parsestruct.Parsejson.CloseParsing(kutsuja+functionname,rememparseuidtwo.ToString());                    
                    } else {
                        this.proghmi.sendError(kutsuja+functionname,"Couldn't find mandatory parameter and couldn't create a correct type of object! Param1:"+mandatoryparam1+" Param2:"+mandatoryparam2,-920,4,4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Returned object was null!",-915,4,4);
                }

                this.parsestruct.Parsejson.CloseParsing(kutsuja+functionname,rememparseuid.ToString());
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Object value was empty!", -917,4,4);
            }

            return retObj;
        }

        /// <summary>
        /// Tämä metodi luo uuden päälaatikon ja sitä vastaavan MotherConnectionRectanglen käyttäen CreateMainBox käskyä, mutta nyt JSON tiedostosta ladattujen tietojen perusteella
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="mainboxuid"> long, mainboxin uid numero, jolle kyseinen kohde luodaan </param>
        /// <param name="ownboxt"> int, laatikon tyyppi, esim. 1=normaali valintaboksi, 2=outside/between boksi, 100=lähtöarvolaatikko jne. </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="jsondictionary">SortedDictionary string string, referenssi listofmotherboxes kohteen JSON objektista luotuun dictionaryyn, josta avataan tietoja</param>
        /// <param name="setleft">double, kohta canvasin vasemmasta reunasta, johon kohti päälaatikko luodaan </param>
        /// <param name="settop">double, kohta canvasin yläreunasta, johon kohti päälaatikko luodaan </param>
        /// <returns> {Rectangle} palauttaa päälaatikolle tehdyn Rectangle muotoisen graafisen elementin. Jos epäonnistui kyseisen objektin luonnissa, niin palauttaa null </returns>
        private Rectangle LoadMainBox(string kutsuja, long mainboxuid, int ownboxt, Canvas canvas, SortedDictionary<string,string> jsondictionary, double setleft, double settop)
        {
            string functionname="->(ACUI)LoadMainBox";
            Rectangle mainbox;

            string mandatoryparam="BlockType";

            if (jsondictionary.ContainsKey(mandatoryparam)==true) {
                if (rectangleDataDict.ContainsKey(ownboxt)==true) {
                    mainbox=this.CreateMainBox(kutsuja+functionname,mainboxuid,ownboxt,canvas,rectangleDataDict[ownboxt].MainBoxWidth,rectangleDataDict[ownboxt].MainBoxHeight,rectangleDataDict[ownboxt].BoxColor,setleft,settop);
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"RectangleDataDictionary do not contain key! Key:"+ownboxt,-930,4,4);
                    return null;
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Mandatory parameter missing! Paramname:"+mandatoryparam,-929,4,4);
                return null;
            }

            return mainbox;
        }        
    
        /// <summary>
        /// Tämä metodi luo mainBox MotherConnectionRectangle objektin pääkomponentiksi sekä siihen liittyvän graafisen komponentin Canvas kanvasille
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="mainboxuid"> long, mainboxin uid numero, jolle kyseinen kohde luodaan </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="boxtype"> int, laatikon tyyppi, esim. 1=normaali valintaboksi, 2=outside/between boksi, 100=lähtöarvolaatikko jne. </param>
        /// <param name="boxcolor"> int, päälaatikon väri indeksinä omasta värilistasta </param>
        /// <param name="textcolor"> int, tekstin väri indeksinä omasta värilistasta </param>
        /// <param name="mainboxwidth"> double, sinisen päälaatikon leveys </param>
        /// <param name="mainboxheight"> double, sinisen päälaatikon korkeus </param>
        /// <param name="setMainBoxLeft"> double, päälaatikon leveysasema luotaessa </param>
        /// <param name="setMainBoxTop"> double, päälaatikon korkeusasema luotaessa </param>
        /// <returns> {Rectangle} palauttaa graafisen komponentin referenssin käyttäjälle, kun on luonut objektin </returns>
        private Rectangle CreateMainBox(string kutsuja, long mainboxuid, int boxtype, Canvas canvas, double mainboxwidth, double mainboxheight, int boxcolor, double setMainBoxLeft, double setMainBoxTop)
        {
            string functionname="->(ACUI)CreateMainBox";
            this.actcenconhan.listofMotherBoxes.Add(mainboxuid,new MotherConnectionRectangle(kutsuja+functionname,mainboxuid,this.actcenconhan.OwnUID, this.actcenconhan.ParentUID, this.parsestruct, this.objindexerref,this.proghmi,this.ReturnRegisteredActionCentreRef,this.parametriprintteri,this.connhandler,this.actcenconhan.listofAllBoxesReferences,this.actcenconhan.listofMotherBoxes,boxtype)); // Laitetaan päälaatikko talteen
            
            Rectangle mainBox = new Rectangle
            {
                Width = mainboxwidth,
                Height = mainboxheight,
                Fill = this.OwnColorList(kutsuja+functionname,boxcolor),
                Stroke = this.OwnColorList(kutsuja+functionname,1),
                StrokeThickness = 2,
                Tag = mainboxuid
            };
            this.actcenconhan.listofMotherBoxes[mainboxuid].mainboxConnectionRect.RectangleObject=mainBox; // Laitetaan laatikon referenssi talteen, jotta saadaan muutettua sen ominaisuuksia jatkossakin
            this.actcenconhan.listofAllBoxesReferences.Add(this.actcenconhan.listofMotherBoxes[mainboxuid].mainboxConnectionRect.OwnUID, this.actcenconhan.listofMotherBoxes[mainboxuid].mainboxConnectionRect); // Laitetaan Motherobjektin laatikon ConnectionRectanglen referenssi suureen listaan talteen 

            // Add to canvas
            Canvas.SetLeft(mainBox, setMainBoxLeft);
            Canvas.SetTop(mainBox, setMainBoxTop);
            canvas.Children.Add(mainBox);

            return mainBox;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="mainboxuid"> long, mainboxin uid numero, jolle kyseinen kohde luodaan </param>
        /// <param name="boxtype"> int, laatikon tyyppi, esim. 1=normaali valintaboksi, 2=outside/between boksi, 100=lähtöarvolaatikko jne. </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>        
        /// <param name="setleft"> double, paikka vasemmalta </param>
        /// <param name="settop"></param>
        /// <returns> {StackPanel} Palauttaa StackPanelin referenssin, johon content panelin tiedot on sisälletytetty. Palauttaa null, jos operaatio epäonnistui </returns>
        private StackPanel LoadContentPanel(string kutsuja, long mainboxuid, int boxtype, Canvas canvas, double setleft, double settop)
        {
            string functionname="->(ACUI)LoadContentPanel";

            StackPanel retObj=null;

            if (rectangleDataDict.ContainsKey(boxtype)==true) {
                retObj=this.CreateContentPanel(kutsuja+functionname,mainboxuid,boxtype,canvas,rectangleDataDict[boxtype].TextColor,setleft,this.SetInfoOffsetLeft,settop,this.SetInfoOffsetTop);
            } else {
                this.proghmi.sendError(kutsuja+functionname,"RectangleDataDictionary didn't contain correct key! Key:"+boxtype,-938,4,4);
            }

            return retObj;
        }

        /// <summary>
        /// Tämä metodi luo päälaatikon sisään stackpanelin, jonka sisään se luo tarvittavat komponentit, jotka halutaan printata päälaatikon päälle
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="mainboxuid"> long, mainboxin uid numero, jolle kyseinen kohde luodaan </param>
        /// <param name="boxtype"> int, laatikon tyyppi, esim. 1=normaali valintaboksi, 2=outside/between boksi, 100=lähtöarvolaatikko jne. </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="textcolor"> int, tekstin väri indeksinä omasta värilistasta </param>
        /// <param name="setMainBoxLeft"> double, päälaatikon leveysasema luotaessa </param>
        /// <param name="setInfoOffsetLeft">double, kuinka paljon infolaatikko on päälaatikon sisällä poikkeutettu päälaatikon vasemmasta reunasta</param>
        /// <param name="setMainBoxTop"> double, päälaatikon korkeusasema luotaessa </param>
        /// <param name="setInfoOffsetTop"> double, kuinka paljon infolaatikko on päälaatikon sisällä poikkeutettu päälaatikon yläreunasta </param>
        /// <returns> {StackPanel} palauttaa graafisen komponentin, jonka sisään asetimme päälaatikon tiedot </returns>
        private StackPanel CreateContentPanel(string kutsuja, long mainboxuid, int boxtype, Canvas canvas, int textcolor, double setMainBoxLeft, double setInfoOffsetLeft, double setMainBoxTop, double setInfoOffsetTop)
        {
            string functionname="->(ACUI)CreateContentPanel";

            StackPanel contentPanel = new StackPanel();

            this.CreateContentBlocks(kutsuja+functionname,mainboxuid,boxtype,textcolor,contentPanel); // Tämä funktio luo sisällön contentPanelille

            contentPanel.Tag=mainboxuid; // InfoPanelin Tagiin kirjoitetaan Parentin uid
            // Add the content panel to the canvas
            Canvas.SetLeft(contentPanel, setMainBoxLeft+setInfoOffsetLeft); // Adjust the position as needed
            Canvas.SetTop(contentPanel, setMainBoxTop+setInfoOffsetTop);
            canvas.Children.Add(contentPanel);

            this.actcenconhan.listofMotherBoxes[mainboxuid].StoredUIcomps.InfoPanel=contentPanel; // Laitetaan tekstikenttien arvot sisältävä graafinen paneeli talteen listoofMotherBoxes tietoikkunaan 

            return contentPanel;
        }

        /// <summary>
        /// Tämä metodi luo päälaatikon oikealle ja vasemmalle puolelle pikkulaatikot, joita voi yhdistää toisiinsa Connection luokan objekteilla
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="mainboxuid">long, mainboxin uid numero, jolle kyseinen kohde on luotu </param>
        /// <param name="mainboxparentuid">long, mainboxin vanhemman uid numero, jolle kyseinen kohde on luotu </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="mainBox"> Rectangle, päälaatikon graafisen komponentin referenssi </param>
        /// <param name="smallboxwidth"> double, päälaatikon sivulle tulevien laatikoiden leveys </param>
        /// <param name="mainboxwidth"> double, sinisen päälaatikon leveys </param>
        /// <param name="smallBoxHeightLeft"> double, pikkulaatikkojen leveys päälaatikon vasemmassa reunassa </param>
        /// <param name="smallBoxHeightRight"> double, pikkulaatikkojen leveys päälaatikon oikeassa reunassa </param>
        /// <param name="letterOffsetLeft"> double, kuinka paljon kirjaimen asemaa on poikkeutettu pikkulaatikkojen osalta sen vasemmasta reunasta </param>
        /// <param name="letterOffsetTop"> double, kuinka paljon kirjaimen asemaa on poikkeutettu pikkulaatikkojen osalta sen yläreunasta </param>
        /// <param name="yellowBoxCount">int, keltaisten laatikoiden määrä.</param>
        /// <param name="yellowBoxLetters">string, kirjaimet, jotka tulostetaan keltaisiin laatikoihin.</param>
        /// <param name="greenBoxCount">int, vihreiden laatikoiden määrä.</param>
        /// <param name="greenBoxLetters">string, kirjaimet, jotka tulostetaan vihreisiin laatikoihin.</param>
        /// <param name="redBoxCount">int, punaisten laatikoiden määrä.</param>
        /// <param name="redBoxLetters">string, kirjaimet, jotka tulostetaan punaisiin laatikoihin.</param>
        /// <returns> {int} palauttaa 1, jos onnistui luomaan kohteet - muussa tapauksessa miinus merkkisen virhearvon</returns>
        private int CreateSmallBoxes(string kutsuja, long mainboxuid, long mainboxparentuid, Canvas canvas, Rectangle mainBox, double smallboxwidth, double mainboxwidth, double smallBoxHeightLeft, double smallBoxHeightRight, double letterOffsetLeft, double letterOffsetTop, int yellowBoxCount, string yellowBoxLetters, int greenBoxCount, string greenBoxLetters, int redBoxCount, string redBoxLetters)
        {
            string functionname="->(ACUI)CreateSmallBoxes";
            ConnectionRectangle tempconnrect;
            long createdobjuid;
            int retVal=-1;
            long bhuid=-1;
            BlockHandles blockhandles=null;

            // Tarkistetaan kirjaimet
            this.CheckAndFillLetters(kutsuja+functionname, ref yellowBoxLetters, yellowBoxCount, "keltaisista");
            this.CheckAndFillLetters(kutsuja+functionname, ref greenBoxLetters, greenBoxCount, "vihreistä");
            this.CheckAndFillLetters(kutsuja+functionname, ref redBoxLetters, redBoxCount, "punaista");           

            MotherConnectionRectangle mothrect = this.objindexerref.GetTypedObject<MotherConnectionRectangle>(kutsuja+functionname,mainboxuid);
            if (mothrect!=null) {
            // Tarkasta tässä, että saimme motherconnectionrectanglen ja ota sieltä blokin uid talteen, jonka jälkeen hae blokin tieto ja luo jokaista connection rectangle objektia (smallboxrectanglesin sisällä) kohti samanindeksinen kohde OperationBlockiin tai mieluummin vielä siten, että käydään OperationBlock kohteesta hakemassa laatikon indeksi ja käytetään sitä indeksiä asettamaan tieto ConnectionRectanglelle 
                Brush color;
                // Create yellow and green boxes
                for (int i = 0; i < yellowBoxCount + greenBoxCount; i++)
                {
                    Brush textcoloria;
                    // Näitä yksityiskohtia on ladattu RectangleDataDict luokasta, jotta oikeanlaiset laatikot saadaan luotua tyypin (boxtype) mukaan
                    if (i<yellowBoxCount) {
                        color=Brushes.Yellow;
                        textcoloria=Brushes.Black;
                        tempconnrect=this.actcenconhan.listofMotherBoxes[mainboxuid].smallBoxRectangles.AddBox(kutsuja+functionname,mainboxuid,mainboxparentuid,(int)ConnectionRectangles.connectionBoxType.YELLOW_BOX_COMPARE_VALUE_1,yellowBoxLetters[i].ToString()); // Tässä ykkönen tarkoittaa keltaista laatikkoa
                        blockhandles=mothrect.ReturnActionCentreRef.GetBlockHandlesByUID(kutsuja+functionname,mothrect.ActionCentreBlockUID);
                        if (tempconnrect != null) {
                            if (blockhandles != null) {
                                bhuid = blockhandles.CreateHandle(kutsuja + functionname, (int)ConnectionRectangles.connectionBoxType.YELLOW_BOX_COMPARE_VALUE_1, i, tempconnrect.OwnUID, tempconnrect.BlockAtomValueRef);
                                if (bhuid >= 0) {
                                    tempconnrect.BlockHandleUIDForReference = bhuid;
                                } else {
                                    retVal=-21;
                                    this.proghmi.sendError(kutsuja + functionname, "Failed to create handle for yellow box! Response:"+bhuid, -1307, 4, 4);
                                }
                            } else {
                                retVal=-22;
                                this.proghmi.sendError(kutsuja + functionname, "Block handles is null for yellow box! ActionCentreBlockUID:"+mothrect.ActionCentreBlockUID, -1308, 4, 4);
                            }
                        } else {
                            retVal=-23;
                            this.proghmi.sendError(kutsuja + functionname, "Failed to add yellow box under MotherRectangle:"+mainboxuid, -1309, 4, 4);
                        }
                    } else {
                        color=Brushes.Green;
                        textcoloria=Brushes.White;
                        tempconnrect=this.actcenconhan.listofMotherBoxes[mainboxuid].smallBoxRectangles.AddBox(kutsuja+functionname,mainboxuid,mainboxparentuid,(int)ConnectionRectangles.connectionBoxType.GREEN_BOX_CHECK_VALUE_2,greenBoxLetters[i - yellowBoxCount].ToString()); // Tässä kakkonen tarkoittaa vihreää laatikkoa
                    
                        blockhandles=mothrect.ReturnActionCentreRef.GetBlockHandlesByUID(kutsuja+functionname,mothrect.ActionCentreBlockUID);
                        if (tempconnrect!=null) {
                            if (blockhandles!=null) {
                                bhuid=blockhandles.CreateHandle(kutsuja+functionname,(int)ConnectionRectangles.connectionBoxType.GREEN_BOX_CHECK_VALUE_2,i-yellowBoxCount,tempconnrect.OwnUID,tempconnrect.BlockAtomValueRef); // i-yellowBoxCount - lähdetään uudessa indeksissä taas nollasta liikkeelle
                                if (bhuid>=0) {
                                    tempconnrect.BlockHandleUIDForReference=bhuid;
                                } else {
                                    retVal=-31;
                                    this.proghmi.sendError(kutsuja + functionname, "Failed to create handle for green box! Response:"+bhuid, -1311, 4, 4);
                                }
                            } else {
                                retVal=-32;
                                this.proghmi.sendError(kutsuja + functionname, "Block handles is null for green box! ActionCentreBlockUID:"+mothrect.ActionCentreBlockUID, -1312, 4, 4);
                            }
                        } else {
                            retVal=-33;
                            this.proghmi.sendError(kutsuja + functionname, "Failed to add yellow box green MotherRectangle:"+mainboxuid, -1313, 4, 4);
                        }                       
                    }

                    createdobjuid=tempconnrect.OwnUID;
                    this.actcenconhan.listofAllBoxesReferences.Add(createdobjuid, tempconnrect); // Lisätään kohde listaan, jossa on kaikki luotujen boksien uid key:nä ja sitten referenssi itse objektiin valuena
                    Rectangle box = CreateSubBox(kutsuja+functionname, canvas, color, smallboxwidth, smallBoxHeightLeft, createdobjuid);
                    tempconnrect.RectangleObject=box;
                    Canvas.SetLeft(box, Canvas.GetLeft(mainBox) + 0);
                    Canvas.SetTop(box, Canvas.GetTop(mainBox) + i * smallBoxHeightLeft);
                    canvas.Children.Add(box);
                    TextBlock tletter = new TextBlock{ Tag = createdobjuid, Foreground = textcoloria };
                    tempconnrect.RectangleObjectText=tletter;
                    Canvas.SetLeft(tletter, Canvas.GetLeft(box) + letterOffsetLeft);
                    Canvas.SetTop(tletter, Canvas.GetTop(box) + letterOffsetTop);
                    canvas.Children.Add(tletter);                
                }

                color=Brushes.Red;
                // Create red boxes
                for (int i = 0; i < redBoxCount; i++)
                {
                    Brush textcolori=Brushes.White;
                    tempconnrect=this.actcenconhan.listofMotherBoxes[mainboxuid].smallBoxRectangles.AddBox(kutsuja+functionname,mainboxuid,mainboxparentuid,(int)ConnectionRectangles.connectionBoxType.RED_BOX_RESULT_VALUE_3,redBoxLetters[i].ToString()); // Tässä kolmonen tarkoittaa punaista laatikkoa
                    blockhandles=mothrect.ReturnActionCentreRef.GetBlockHandlesByUID(kutsuja+functionname,mothrect.ActionCentreBlockUID);
                    if (tempconnrect!=null) {
                        if (blockhandles!=null) {
                            bhuid=blockhandles.CreateHandle(kutsuja+functionname,(int)ConnectionRectangles.connectionBoxType.GREEN_BOX_CHECK_VALUE_2,i-yellowBoxCount,tempconnrect.OwnUID,tempconnrect.BlockAtomValueRef); // i-yellowBoxCount - lähdetään uudessa indeksissä taas nollasta liikkeelle
                            if (bhuid>=0) {
                                tempconnrect.BlockHandleUIDForReference=bhuid;
                            } else {
                                retVal=-41;
                                this.proghmi.sendError(kutsuja + functionname, "Failed to create handle for red box! Response:"+bhuid, -1314, 4, 4);
                            }
                        } else {
                            retVal=-42;
                            this.proghmi.sendError(kutsuja + functionname, "Block handles is null for red box! ActionCentreBlockUID:"+mothrect.ActionCentreBlockUID, -1315, 4, 4);
                        }
                    } else {
                        retVal=-43;
                        this.proghmi.sendError(kutsuja + functionname, "Failed to add yellow box red MotherRectangle:"+mainboxuid, -1316, 4, 4);
                    }  

                    createdobjuid=tempconnrect.OwnUID;
                    this.actcenconhan.listofAllBoxesReferences.Add(createdobjuid, tempconnrect); // Lisätään kohde listaan, jossa on kaikki luotujen boksien uid key:nä ja sitten referenssi itse objektiin valuena               
                    Rectangle box = CreateSubBox(kutsuja+functionname, canvas, color, smallboxwidth, smallBoxHeightRight,createdobjuid);
                    tempconnrect.RectangleObject=box;
                    Canvas.SetLeft(box, Canvas.GetLeft(mainBox) + mainboxwidth - smallboxwidth);
                    Canvas.SetTop(box, Canvas.GetTop(mainBox) + i * smallBoxHeightRight);
                    canvas.Children.Add(box);
                    TextBlock tletter = new TextBlock{ Tag = createdobjuid, Foreground = textcolori };
                    tempconnrect.RectangleObjectText=tletter;
                    Canvas.SetLeft(tletter, Canvas.GetLeft(box) + letterOffsetLeft);
                    Canvas.SetTop(tletter, Canvas.GetTop(box) + letterOffsetTop);
                    canvas.Children.Add(tletter);
                }
                retVal=1;
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Couldn't get MotherConnectionRectangle through ObjectIndexer! MainboxUID:"+mainboxuid+" ParentUid:"+mainboxparentuid,-1294,4,4);
                retVal=-10;
                return retVal;
            }
            return retVal;
        }

        /// <summary>
        /// Tämä metodi liittää päälaatikon graafiselle objektille sen tarvitsemat event handlerit
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="mainboxuid">long, mainboxin uid numero, jolle kyseinen kohde on luotu </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="mainBox"> Rectangle, päälaatikon graafisen komponentin referenssi </param>
        /// <param name="mainboxwidth"> double, sinisen päälaatikon leveys </param>
        /// <param name="smallboxwidth"> double, päälaatikon sivulle tulevien laatikoiden leveys </param>
        /// <param name="smallBoxHeightLeft"> double, pikkulaatikkojen leveys päälaatikon vasemmassa reunassa </param>
        /// <param name="smallBoxHeightRight"> double, pikkulaatikkojen leveys päälaatikon oikeassa reunassa </param>
        /// <param name="setInfoOffsetLeft">double, kuinka paljon infolaatikko on päälaatikon sisällä poikkeutettu päälaatikon vasemmasta reunasta</param>
        /// <param name="setInfoOffsetTop"> double, kuinka paljon infolaatikko on päälaatikon sisällä poikkeutettu päälaatikon yläreunasta </param>        
        /// <param name="letterOffsetLeft"> double, kuinka paljon kirjaimen asemaa on poikkeutettu pikkulaatikkojen osalta sen vasemmasta reunasta </param>
        /// <param name="letterOffsetTop"> double, kuinka paljon kirjaimen asemaa on poikkeutettu pikkulaatikkojen osalta sen yläreunasta </param>
        /// <returns> {void} </returns>
        private void SetMainBoxEventHandlers(string kutsuja, long mainboxuid, Canvas canvas, Rectangle mainBox, double mainboxwidth, double smallboxwidth, double smallBoxHeightLeft, double smallBoxHeightRight, double setInfoOffsetLeft, double setInfoOffsetTop, double letterOffsetLeft, double letterOffsetTop)
        {
            string functionname="->(ACUI)SetMainBoxEventHandlers";
            // Eventit rekisteröitynä graafisille objekteille
            double deltaX = 0;
            double deltaY = 0;
            bool isDragging = false;

            mainBox.MouseDown += (sender, e) =>
            {
                if (isDeleting)
                {
                    // Tähän kohtaan lisää koodisi objektin poistoa varten
                    this.actcenconhan.listofMotherBoxes[mainboxuid].DeleteMotherConnectionRectangleComponents(kutsuja+functionname,canvas);
                } else {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        var position = e.GetPosition(mainBox);
                        deltaX = position.X;
                        deltaY = position.Y;
                        isDragging = true;
                        mainBox.CaptureMouse();
                    }
                }
            };

            mainBox.MouseMove += (sender, e) =>
            {
                if (isDragging)
                {
                    var position = e.GetPosition(canvas);
                    double newLeft = position.X - deltaX;
                    double newTop = position.Y - deltaY;
                    this.actcenconhan.listofMotherBoxes[mainboxuid].UpdatePositionsDuringDrag(kutsuja+functionname,newTop,newLeft,mainboxwidth,smallboxwidth,smallBoxHeightLeft,smallBoxHeightRight,setInfoOffsetLeft,setInfoOffsetTop, letterOffsetLeft, letterOffsetTop);

                    //foreach (var connection in this.connhandler.connections)
                    //{
                    //    this.connhandler.UpdateConnection(connection.Value);
                    //}
                    //foreach (var connection in connections.Where(c => c.Box1 == mainBox || c.Box2 == mainBox))
                    //{
                    //    UpdateConnection(connection);
                    //}                                                          
                }                
            };

            mainBox.MouseUp += (sender, e) =>
            {
                isDragging = false;
                mainBox.ReleaseMouseCapture();

            };
        }

        /// <summary>
        /// Uusi funktio, joka käyttää rectangleDataDict Dictionary tietorakennetta ja kutsuu AddRectangle-metodia luodakseen objektit
        /// </summary>
        /// <param name="caller">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="boxtype"> int, laatikon tyyppi, esim. 1=normaali valintaboksi, 2=outside/between boksi, 100=lähtöarvolaatikko jne. </param>
        /// <returns> {void} Sisäinen palautus luku on 1, jos onnistui, -13=jos ei löytynyt kohdetta (RectangleData) ja -2=jos virhe alifunktiossa </returns>
        private void AddRectangleByType(string caller, Canvas canvas, int boxType)
        {
            string functionname="->(ACUI)AddRectangleByType";
            int retVal=-1;
            // Tarkistetaan, löytyykö boxType arvo rectangleDataDict:stä
            if (!rectangleDataDict.TryGetValue(boxType, out RectangleData rectData))
            {
                retVal=-13;
                // Jos ei löydy, annetaan virheilmoitus ja palautetaan null
                this.proghmi.sendError(caller+functionname, "Invalid boxType: " + boxType+" Response:"+retVal, -922, 4, 4);
            } else {

                // Kutsutaan AddRectangle-metodia käyttäen RectangleData-instanssin tietoja
                retVal=this.AddRectangle(
                    caller+functionname, 
                    canvas, 
                    rectData.BoxType, 
                    rectData.BoxColor, 
                    rectData.TextColor, 
                    rectData.MainBoxWidth, 
                    rectData.MainBoxHeight, 
                    rectData.SmallBoxWidth, 
                    rectData.YellowBoxCount, 
                    rectData.YellowBoxLetters, 
                    rectData.GreenBoxCount, 
                    rectData.GreenBoxLetters, 
                    rectData.RedBoxCount, 
                    rectData.RedBoxLetters
                );
                if (retVal<0) {
                    this.proghmi.sendError(caller+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+retVal+" OwnUID:"+this.OwnUID+" ParentUid:"+this.ParentUID,-1079,4,4);
                }
            }
            
        }

        /// <summary>
        /// Uusi funktio, joka käyttää rectangleDataDict Dictionary tietorakennetta ja kutsuu AddRectangleSubMethods-metodia
        /// </summary>
        /// <param name="callerFunctionName">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="boxtype"> int, laatikon tyyppi, esim. 1=normaali valintaboksi, 2=outside/between boksi, 100=lähtöarvolaatikko jne. </param>
        /// <param name="motherrectuid"> long, MotherConnectionRectangle objektin uid, joka toimii äitiobjektina tällä metodilla luoduille alaobjekteille (nähtävästi pikkulaatikoille) </param>
        /// <returns> {void} </returns>
        private void AddRectangleSubMethodsByType(string callerFunctionName, Canvas canvas, int boxType, Rectangle mainBox, long motherrectuid)
        {
            string functionname="->(ACUI)AddRectangleSubMethodsByType";
            // Tarkistetaan, löytyykö boxType arvo rectangleDataDict:stä
            if (!rectangleDataDict.TryGetValue(boxType, out RectangleData rectData))
            {
                // Jos ei löydy, annetaan virheilmoitus ja palautetaan null
                this.proghmi.sendError(callerFunctionName+functionname, "Invalid boxType: " + boxType, -939, 4, 4);
            } else {

                if (this.objindexerref.objectlist.IndexOfKey(motherrectuid)>-1) {
                    long motherrectparentuid=this.objindexerref.objectlist[motherrectuid].ParentUID;

                    if (motherrectparentuid>=0) {
                        // Kutsutaan AddRectangle-metodia käyttäen RectangleData-instanssin tietoja
                        this.AddRectangleSubMethods(
                            callerFunctionName+functionname,
                            motherrectuid,
                            motherrectparentuid,
                            mainBox, 
                            canvas,  
                            rectData.MainBoxWidth,  
                            rectData.SmallBoxWidth, 
                            rectData.YellowBoxCount, 
                            rectData.YellowBoxLetters, 
                            rectData.GreenBoxCount, 
                            rectData.GreenBoxLetters, 
                            rectData.RedBoxCount, 
                            rectData.RedBoxLetters
                        );
                    } else {
                        this.proghmi.sendError(callerFunctionName+functionname, "Couldn't find object parent uid in object indexer - UID:"+motherrectuid+" Boxtype:" + boxType+" MotherrectParentUID:"+motherrectparentuid, -1286, 4, 4);
                    }
                } else {
                    this.proghmi.sendError(callerFunctionName+functionname, "Couldn't find object in object indexer - UID:"+motherrectuid+" Boxtype:" + boxType, -1285, 4, 4);
                }
            }
        }               

        /// <summary> Luo suorakulmion Canvasin sisälle, jota voi vetää hiirellä toiseen paikkaan </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="boxtype"> int, laatikon tyyppi, esim. 1=normaali valintaboksi, 2=outside/between boksi, 100=lähtöarvolaatikko jne. </param>
        /// <param name="boxcolor"> int, päälaatikon väri indeksinä omasta värilistasta </param>
        /// <param name="textcolor"> int, tekstin väri indeksinä omasta värilistasta </param>
        /// <param name="mainboxwidth"> double, sinisen päälaatikon leveys </param>
        /// <param name="mainboxheight"> double, sinisen päälaatikon korkeus </param>
        /// <param name="smallboxwidth"> double, päälaatikon sivulle tulevien laatikoiden leveys </param>
        /// <param name="yellowBoxCount">int, keltaisten laatikoiden määrä.</param>
        /// <param name="yellowBoxLetters">string, kirjaimet, jotka tulostetaan keltaisiin laatikoihin.</param>
        /// <param name="greenBoxCount">int, vihreiden laatikoiden määrä.</param>
        /// <param name="greenBoxLetters">string, kirjaimet, jotka tulostetaan vihreisiin laatikoihin.</param>
        /// <param name="redBoxCount">int, punaisten laatikoiden määrä.</param>
        /// <param name="redBoxLetters">string, kirjaimet, jotka tulostetaan punaisiin laatikoihin.</param>
        /// <returns> {int} palauttaa 1, jos laatikon lisäys onnistui ja pienemmän luvun kuin 0, jos virhe. -1=määrittelemätön virhe, -2=virhe UID tietojen antamisessa </returns>
        private int AddRectangle(string kutsuja, Canvas canvas, int boxtype, int boxcolor, int textcolor, double mainboxwidth, double mainboxheight, double smallboxwidth, int yellowBoxCount, string yellowBoxLetters, int greenBoxCount, string greenBoxLetters, int redBoxCount, string redBoxLetters)
        {
            string functionname="->(ACUI)AddRectangle";
            int retVal=-1;

            // TODO: Tähän alapuoliselle riville pitää kehittää varmaan jotain tuon ensimmäisen -1 korvaamaan. Esim. näkymän UID tms. parent arvona 
            // Luo MotherConnectionRectanglelle yksilöllisen UID arvon
            long uid = this.objindexerref.AddObjectToIndexer(kutsuja+functionname,this.actcenconhan.OwnUID,(int)ObjectIndexer.indexerObjectTypes.MOTHER_COMPONENT_RECTANGLE_102,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.actcenconhan.ParentUID); 

            if (uid>=0) {
                // luo mainBox MotherConnectionRectangle objektin pääkomponentiksi sekä siihen liittyvän graafisen komponentin Canvas kanvasille
                Rectangle mainBox=this.CreateMainBox(kutsuja+functionname,uid,boxtype,canvas,mainboxwidth,mainboxheight,boxcolor,this.SetMainBoxLeft, this.SetMainBoxTop);

                StackPanel contentPanel=this.CreateContentPanel(kutsuja+functionname,uid,boxtype,canvas,textcolor,this.SetMainBoxLeft,this.SetInfoOffsetLeft,this.SetMainBoxTop, this.SetInfoOffsetTop); // Luodaan sisältö Mainboxin sisään 

                this.AddRectangleSubMethods(kutsuja+functionname, uid, this.actcenconhan.OwnUID,mainBox, canvas, mainboxwidth, smallboxwidth, yellowBoxCount, yellowBoxLetters, greenBoxCount, greenBoxLetters, redBoxCount, redBoxLetters);
                retVal=1;
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+uid+" ParentUid:"+this.actcenconhan.OwnUID,-1078,4,4);
                retVal=-2;
            }
            return retVal;           
        }

        /// <summary> Luo pienet suorakulmiot mainbox rectanglen sisälle sekä yhdistää kohteisiin Eventhandlerit </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="uid"> long, yksilöllinen luotavan MotherconnectionRectanglen uid </param>
        /// <param name="parentuid"> long, yksilöllinen luotavan MotherconnectionRectanglen äitiobjektin uid </param>
        /// <param name="mainBox"> Rectangle, päälaatikon referenssi, jonka alle graafisesti pikkulaatikoida luodaan </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <param name="mainboxwidth"> double, sinisen päälaatikon leveys </param>
        /// <param name="mainboxheight"> double, sinisen päälaatikon korkeus </param>
        /// <param name="smallboxwidth"> double, päälaatikon sivulle tulevien laatikoiden leveys </param>
        /// <param name="yellowBoxCount">int, keltaisten laatikoiden määrä.</param>
        /// <param name="yellowBoxLetters">string, kirjaimet, jotka tulostetaan keltaisiin laatikoihin.</param>
        /// <param name="greenBoxCount">int, vihreiden laatikoiden määrä.</param>
        /// <param name="greenBoxLetters">string, kirjaimet, jotka tulostetaan vihreisiin laatikoihin.</param>
        /// <param name="redBoxCount">int, punaisten laatikoiden määrä.</param>
        /// <param name="redBoxLetters">string, kirjaimet, jotka tulostetaan punaisiin laatikoihin.</param>
        /// <returns> {void} </returns>
        private void AddRectangleSubMethods(string kutsuja, long uid, long parentuid, Rectangle mainBox, Canvas canvas, double mainboxwidth, double smallboxwidth, int yellowBoxCount, string yellowBoxLetters, int greenBoxCount, string greenBoxLetters, int redBoxCount, string redBoxLetters)
        {
            string functionname="->(ACUI)AddRectangleSubMethods";
            double letterOffsetLeft=1;
            double letterOffsetTop=1;

            // Calculate the height for the smaller boxes
            double smallBoxHeightLeft = mainBox.Height / (yellowBoxCount + greenBoxCount);
            double smallBoxHeightRight = mainBox.Height / redBoxCount;

            // Luo pikku laatikot molemmille puolille isoa laatikkoa
            this.CreateSmallBoxes(kutsuja+functionname,uid,parentuid,canvas,mainBox,smallboxwidth,mainboxwidth,smallBoxHeightLeft,smallBoxHeightRight,letterOffsetLeft,letterOffsetTop,yellowBoxCount,yellowBoxLetters,greenBoxCount,greenBoxLetters,redBoxCount,redBoxLetters);

            // Asettaa päälaatikolle event-handlerit
            this.SetMainBoxEventHandlers(kutsuja+functionname,uid, canvas,mainBox,mainboxwidth,smallboxwidth,smallBoxHeightLeft,smallBoxHeightRight,this.SetInfoOffsetLeft,this.SetInfoOffsetTop, letterOffsetLeft, letterOffsetTop);
        }

        /// <summary>
        /// Tämä funktio disabloi/enabloi muut napit kesken moniportaisten nappien painamiskäskyjen
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="enablestate"> bool, enabloidaanko vai disabloidaanko napin käyttö </param>
        /// <param name="activebutton">Button, se nappi jota ollaan sillä hetkellä parhaillaan käyttämässä - kaikki muut napit disabloidaan</param>
        private void SetDisableEnableButtons(string kutsuja, bool enablestate, Button activebutton)
        {
            if (activebutton!=connectButton) {
                connectButton.IsEnabled=!enablestate;
            }
            if (activebutton!=disconnectButton) {
                disconnectButton.IsEnabled=!enablestate;
            }
            if (activebutton!=deleteButton) {
                deleteButton.IsEnabled=!enablestate;
            }      
        }

        /// <summary>
        /// Tämä metodi luo olemassa olevien komponenttien perusteella tietorakenteen, jonka perusteella ajaa itse step enginellä kohteet läpi
        /// TODO: Tämä käsky luo vain StepEngineInstruction:it kyseisen luokan instanssille, eikä koko StepEngine:n tietorakennetta kerralla. Korjaa tämä puute!
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {void} </returns>
        public void CreateInstructionsForStepEngine(string kutsuja)
        {
            string functionname="->(ACUI)CreateInstructionsForStepEngine";
            // TODO: Tämä käsky luo vain StepEngineInstruction:it kyseisen luokan instanssille, eikä koko StepEngine:n tietorakennetta kerralla. Korjaa tämä puute!

            StepEngineInstruction stepenginst=this.stepeng.CreateNewInstruction(kutsuja+functionname);

            if (stepenginst!=null) {
                // TODO: Otetaan ylös juuri luodun stepengine instructionin UID tiedot, mutta jatkossa kyseessä ei ole vain yksittäiset kohteet, vaan kokoelma kohteita, joita on käytävä läpi yksittäisen kohteen sijasta
                this.StepEngineInstructionUID=stepenginst.OwnUID;
                this.StepEngineSuperBlockUID=stepenginst.ParentUID;

                // Käydään jokainen MotherRectangle läpi ja tallennetaan kaikki lähtöarvokohteet tietolistaksi
                int amo=this.actcenconhan.listofMotherBoxes.Count;
                int i=-1;
                if (amo>0) {
                    for (i=0; i<amo; i++) { // Käydään jokainen objekti läpi
                        // Jos on kohteita, joista käsin aloitetaan aina kohteiden läpikäynti
                        if (this.actcenconhan.listofMotherBoxes.ElementAt(i).Value.BlockType==(int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100 || this.actcenconhan.listofMotherBoxes.ElementAt(i).Value.BlockType==(int)ActionCentre.blockTypeEnum.OWN_VALUE_BLOCK_101) {
                            stepenginst.blocksstartingkeylist.Add(this.actcenconhan.listofMotherBoxes.ElementAt(i).Key,this.actcenconhan.listofMotherBoxes.ElementAt(i).Value.BlockType); // Otetaan talteen lähtöblokkien UID sekä tyyppi
                        }
                        stepenginst.allblockskeylist.Add(this.actcenconhan.listofMotherBoxes.ElementAt(i).Key,this.actcenconhan.listofMotherBoxes.ElementAt(i).Value.BlockType); // Otetaan talteen kaikkien blokkien UID sekä tyyppi
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"No components added to screen! Add at least one! Instructions creation halted!",763,2,4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Couldn't create a new instruction! Instruction was null!",-1031,4,4);
            }
        }

        /// <summary>
        /// Tämä metodi ajaa olemassa olevien komponenttien perusteella tietorakenteen kohteet stepenginellä läpi
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="altrouteval"> int, ajettavan slottilistan kohteen altrouteval tieto, josta käsin jatketaan kohteen ajoa </param>
        /// <param name="oneSlot"> OneSlot, sen luokan instanssin referenssi, jonka tiedot on ajettava blokkikonstruktiossa </param>
        /// <returns> {int} Palauttaa AltRoute arvon, johon saakka järjestelmä pääsi blokkirakenteen ajamisessa </returns>
        public int RunStepEngineInstructions(string kutsuja, OneSlot oneSlot)
        {
            string functionname="->(ACUI)RunStepEngineInstructions";
            StepEngineInstruction stepenginstref;

            stepenginstref=this.stepeng.FindInstructionByUID(kutsuja+functionname,this.StepEngineSuperBlockUID, this.StepEngineInstructionUID); // Etsitään, että mitä StepEngineInstruction luokkaa käytetään
            if (stepenginstref!=null) {
                if (oneSlot.blockinstructobj.attemptobjects.Count>0) {
                    foreach (var attemptObj in oneSlot.blockinstructobj.attemptobjects.Values) {
                        if (attemptObj.IsOldMode == 1) {
                            if (attemptObj.AlternativeRouteHandler.altRoutes.Count>0) { // Jos meillä on jo merkattuna useita reittejä altRoutes listaan, joiden edistymistä tulee tutkia
                                foreach (var altRouteStep in attemptObj.AlternativeRouteHandler.altRoutes.Values)
                                {
                                    int newAltRoute = stepenginstref.RunInstruction(kutsuja+functionname, altRouteStep.AltRoute, altRouteStep.AltRouteObjectUID, oneSlot);
                                    if (newAltRoute != -100)
                                    {
                                        altRouteStep.AltRoute = newAltRoute;
                                    }
                                }
                            } else {
                                // Jos yhtään kohdetta ei ole listalla, niin tällöin meidän tulee ottaa aloituskohteet StepEngineInstructions luokan blocksstartingkeylist listalta ja 
                                // lähteä niistä objekteista käsin ajamaan blokkeja sekä tallentamaan niiden tietoja altRouteStep objekteihin

                            }
                        }
                    }
                }
            } else {

            }
        }  

        /// <summary> Tämä avattava pop-up ikkuna on ActionCentren komponenttien rakennus ikkuna </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {void} </returns>
        public void OpenActionCentreWindow(string kutsuja)
        {
            string functionname="->(ACUI)OpenActionCentreWindow";

            long popupuid=this.CreateActionCentreUIPopupWindow(kutsuja+functionname);
        }

        /// <summary> Tämä luo pop-up ikkunan joka on ActionCentren komponenttien rakennus ikkuna </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {long} Palauttaa luodun popup ikkunan objectindexer UID koodin. Jos palauttaa pienemmän kuin 0, niin virhe </returns>
        public long CreateActionCentreUIPopupWindow(string kutsuja)
        {
            string functionname="->(ACUI)CreateActionCentreUIPopupWindow";
            long retVal=-1;

            if (this.IsClassInitialized==true) {
                retVal = this.objindexerref.AddObjectToIndexer(kutsuja+functionname,this.OwnUID,(int)ObjectIndexer.indexerObjectTypes.ACTIONCENTREUI_POPUP_WINDOW_301,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1); // TODO: Tässä ensimmäinen -1 on parentin UID, joka pitäisi korvata jollain. Nyt -1 tarkoittaa, että tämän yläpuolella ei ole mitään, vaikka oikeasti on se templaatti, johon tämä komponentti kuuluu
                
                if (retVal>=0) {
                    // Luodaan uusi ikkuna                
                    Window popupWindow = new Window()
                    {
                        Width = 1525,
                        Height = 750,
                        Title = "Action Centre",
                        Tag = retVal
                    };

                    // Luodaan Grid
                    Grid grid = new Grid();
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    // Luodaan Canvas-alue
                    Canvas canvas = this.canvasinuse;

                    // Luodaan nappien rivi
                    WrapPanel buttonPanel = new WrapPanel { 
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(1),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top                     
                    };
                    this.lisaaValintaButton = new Button { Content = "Lisää valinta", Margin = new Thickness(5) };
                    // Sitten "Lisää valinta" napin Click event handler:
                    lisaaValintaButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_NORMAL_IF_1);
                    //lisaaValintaButton.Click += (sender, e) => AddRectangle(kutsuja+functionname,canvas,(int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_NORMAL_IF_1,10,9,blockboxwidth,115,blockconnectionwidth,1,"I",1,"C",3,"RTF");
                    buttonPanel.Children.Add(lisaaValintaButton);

                    this.betweenButton = new Button { Content = "Lisää between/outside", Margin = new Thickness(5) };
                    //betweenButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_BETWEEN_OUTSIDE_2,10,9,blockboxwidth, 115, blockconnectionwidth, 2,"HL", 1,"C", 3,"RTF");
                    betweenButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_BETWEEN_OUTSIDE_2);
                    buttonPanel.Children.Add(betweenButton);               

                    this.lahtoarvoButton = new Button { Content = "Lisää lähtöarvo", Margin = new Thickness(5) };
                    //lahtoarvoButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100, 12,1, blockboxwidth, 115, blockconnectionwidth, 0,"", 0,"", 1,"V");
                    lahtoarvoButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100);
                    buttonPanel.Children.Add(lahtoarvoButton);                

                    this.operaatio2Button = new Button { Content = "Lisää 2:n operaatio", Margin = new Thickness(5) };
                    //operaatio2Button.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_200, 3,1, blockboxwidth, 115, blockconnectionwidth, 1,"O", 1,"I", 1,"R");
                    operaatio2Button.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_200);
                    buttonPanel.Children.Add(operaatio2Button);             

                    this.operaatio3Button = new Button { Content = "Lisää 3:n operaatio", Margin = new Thickness(5) };
                    //operaatio3Button.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_3_PROPERTY_201, 3,1, blockboxwidth, 115, blockconnectionwidth, 2,"OO", 1,"I", 1,"R");
                    operaatio3Button.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_3_PROPERTY_201);
                    buttonPanel.Children.Add(operaatio3Button);                             

                    this.kahvaInButton = new Button { Content = "Lisää kahva IN", Margin = new Thickness(5) };
                    //kahvaInButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.HANDLE_BLOCK_IN_300, 4,1, blockboxwidth, 115, blockconnectionwidth, 0,"", 0,"", 1,"O");
                    kahvaInButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.HANDLE_BLOCK_IN_300);
                    buttonPanel.Children.Add(kahvaInButton);

                    this.kahvaOutButton = new Button { Content = "Lisää kahva OUT", Margin = new Thickness(5) };
                    //kahvaOutButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.HANDLE_BLOCK_OUT_301, 4,1, blockboxwidth, 115, blockconnectionwidth, 1,"I", 0,"", 0,"");
                    kahvaOutButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.HANDLE_BLOCK_OUT_301);
                    buttonPanel.Children.Add(kahvaOutButton);                 

                    this.marketBuyButton = new Button { Content = "Market Buy", Margin = new Thickness(5) };
                    //marketBuyButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.MARKET_BUY_BLOCK_400, 11,0, blockboxwidth, 115, blockconnectionwidth, 1,"S", 1,"L", 3,"USE");
                    marketBuyButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.MARKET_BUY_BLOCK_400);
                    buttonPanel.Children.Add(marketBuyButton); 

                    this.marketSellButton = new Button { Content = "Market Sell", Margin = new Thickness(5) };
                    //marketSellButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.MARKET_SELL_BLOCK_401, 5,1, blockboxwidth, 115, blockconnectionwidth, 1,"S", 1,"L", 3,"USE");
                    marketSellButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.MARKET_SELL_BLOCK_401);
                    buttonPanel.Children.Add(marketSellButton);

                    this.limitBuyButton = new Button { Content = "Limit Buy", Margin = new Thickness(5) };
                    //limitBuyButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.LIMIT_BUY_BLOCK_402, 7,0, blockboxwidth, 115, blockconnectionwidth, 2,"SC", 1,"L", 3,"USE");
                    limitBuyButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.LIMIT_BUY_BLOCK_402);
                    buttonPanel.Children.Add(limitBuyButton);

                    this.limitSellButton = new Button { Content = "Limit Sell", Margin = new Thickness(5) };
                    //limitSellButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.LIMIT_SELL_BLOCK_403, 6,0, blockboxwidth, 115, blockconnectionwidth, 2,"SC", 1, "L", 3,"USE");
                    limitSellButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.LIMIT_SELL_BLOCK_403);
                    buttonPanel.Children.Add(limitSellButton);

                    this.testIfFilledButton = new Button { Content = "Test If Filled", Margin = new Thickness(5) };
                    //testIfFilledButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.TEST_IF_FILLED_BLOCK_500, 2,0, blockboxwidth, 115, blockconnectionwidth, 1,"U", 1,"L", 5,"RTPFE");
                    testIfFilledButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.TEST_IF_FILLED_BLOCK_500);
                    buttonPanel.Children.Add(testIfFilledButton);

                    this.testIfRemovedFromListButton = new Button { Content = "Test If Removed", Margin = new Thickness(5) };
                    //testIfRemovedFromListButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.TEST_IF_REMOVED_MOWWM_BLOCK_600, 14,1, blockboxwidth, 115, blockconnectionwidth, 1, "U", 1, "L", 2, "RTF");
                    testIfRemovedFromListButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.TEST_IF_REMOVED_MOWWM_BLOCK_600);
                    buttonPanel.Children.Add(testIfRemovedFromListButton);

                    this.resetBlocksButton = new Button { Content = "Reset Blocks", Margin = new Thickness(5) };
                    //resetBlocksButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.RESET_ALL_BLOCK_VALUES_700, 15,1, blockboxwidth, 115, blockconnectionwidth, 0,"", 1,"C", 1,"C");
                    resetBlocksButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.RESET_ALL_BLOCK_VALUES_700);
                    buttonPanel.Children.Add(resetBlocksButton);

                    this.endForNowButton = new Button { Content = "End Now ", Margin = new Thickness(5) };
                    //endForNowButton.Click += (sender, e) => AddRectangle(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.END_FOR_NOW_BLOCK_800, 16,0, blockboxwidth, 115, blockconnectionwidth, 0,"", 1,"C", 0,"");
                    endForNowButton.Click += (sender, e) => AddRectangleByType(kutsuja + functionname, canvas, (int)ActionCentre.blockTypeEnum.END_FOR_NOW_BLOCK_800);
                    buttonPanel.Children.Add(endForNowButton);    
                    
                    this.connectButton = new Button { Content = "Lisää yhteys", Margin = new Thickness(5)};
                    connectButton.Click += (sender, e) =>
                    {
                        isConnecting = !isConnecting;
                        connectButton.Content = isConnecting ? "Paina peruuttaaksesi" : "Lisää yhteys";
                        connectionStatusText.Text = isConnecting ? "Paina 1. laatikkoa" : "";
                        firstSelectedBox = null;
                        secondSelectedBox = null;

                        // Vaihda muiden nappien tilaa poistotilan mukaan (vapaaehtoista, mutta voi parantaa käytettävyyttä)
                        this.SetDisableEnableButtons(kutsuja+functionname,isConnecting,connectButton);
                        //disconnectButton.IsEnabled=!isConnecting;
                        //deleteButton.IsEnabled=!isConnecting;
                    };
                    buttonPanel.Children.Add(connectButton);

                    // Luo ComboBox
                    this.colorComboBox = new ComboBox
                    {
                        Width = 70
                    };
                    // Lisää värit
                    this.AddColorsToComboBox(kutsuja+functionname);

                    // Lisää tapahtumankäsittelijä
                    colorComboBox.SelectionChanged += ColorComboBox_SelectionChanged;

                    // Sijoita ComboBox lomakkeelle (esim. jos käytät Canvasia nimeltä "canvas")
                    buttonPanel.Children.Add(colorComboBox);           

                    // Muuta "Poista yhteys" -painikkeen tapahtumankäsittelijä
                    this.disconnectButton = new Button { Content = "Poista yhteys", Margin = new Thickness(5) };
                    disconnectButton.Click += (sender, e) =>
                    {
                        isDisconnecting = !isDisconnecting;
                        disconnectButton.Content = isDisconnecting ? "Peruuta yhteyden poisto" : "Poista yhteys";
                        connectionStatusText.Text = isDisconnecting ? "Paina 1. laatikkoa" : "Yhteyden poisto peruttu";
                        firstSelectedBox = null;
                        secondSelectedBox = null;

                        // Vaihda muiden nappien tilaa poistotilan mukaan (vapaaehtoista, mutta voi parantaa käytettävyyttä)
                        this.SetDisableEnableButtons(kutsuja+functionname,isDisconnecting,disconnectButton);
                    };
                    buttonPanel.Children.Add(disconnectButton);

                    // Luo "Poista objekti" -nappi
                    deleteButton = new Button { Content = "Poista objekti", Margin = new Thickness(5) };
                    deleteButton.Click += (sender, e) =>
                    {
                        isDeleting = !isDeleting; // Vaihda poistotilan tilaa
                        deleteButton.Content = isDeleting ? "Peruuta poisto" : "Poista objekti";
                        
                        // Vaihda muiden nappien tilaa poistotilan mukaan (vapaaehtoista, mutta voi parantaa käytettävyyttä)
                        this.SetDisableEnableButtons(kutsuja+functionname,isDeleting,deleteButton);
                    };
                    buttonPanel.Children.Add(deleteButton);

                    this.createInstructionsButton=new Button { Content = "Create Instructions", Margin = new Thickness(5)};
                    createInstructionsButton.Click += (sender, e) =>
                    {
                        this.CreateInstructionsForStepEngine(kutsuja+functionname);
                    };
                    buttonPanel.Children.Add(this.createInstructionsButton);

                    this.loadComponentButton = new Button { Content = "Lataa komponentti", Margin = new Thickness(5) };
                    this.loadComponentButton.Click += (sender, e) => LoadComponentInformation(kutsuja+functionname);
                    buttonPanel.Children.Add(this.loadComponentButton);
                    
                    this.saveComponentButton = new Button { Content = "Tallenna komponentti", Margin = new Thickness(5) };
                    this.saveComponentButton.Click += (sender, e) => SaveComponentInformation(kutsuja+functionname);
                    buttonPanel.Children.Add(this.saveComponentButton);

                    buttonPanel.Children.Add(new Button { Content = "Poistu", Margin = new Thickness(5) });
                    Grid.SetRow(buttonPanel, 0);
                    grid.Children.Add(buttonPanel);

                    // Luodaan TextBoxit
                    StackPanel textBoxPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    textBoxPanel.Children.Add(new TextBlock { Text = "UID:" });
                    long wholecomptextboxuid=this.objindexerref.AddObjectToIndexer(kutsuja+functionname,this.OwnUID,(int)ObjectIndexer.indexerObjectTypes.UI_COMPONENT_TEXTBOX_201,-1);
                    if (wholecomptextboxuid>=0) {
                        this.wholeComponentUID = new TextBox { Width = 200, Margin = new Thickness(5) };
                        this.wholeComponentUID.Tag = wholecomptextboxuid;
                        this.wholeComponentUID.Text = retVal.ToString();
                        textBoxPanel.Children.Add(this.wholeComponentUID);
                    } else {
                        this.proghmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+wholecomptextboxuid+" ParentUid:"+this.ParentUID+" OwnUID:"+this.OwnUID,-1081,4,4);
                    }                         

                    long wholecompnameuid=this.objindexerref.AddObjectToIndexer(kutsuja+functionname,this.OwnUID,(int)ObjectIndexer.indexerObjectTypes.UI_COMPONENT_TEXTBOX_201,-1);
                    if (wholecompnameuid>=0) {
                        textBoxPanel.Children.Add(new TextBlock { Text = "Comp Name:" });
                        this.wholeComponentName = new TextBox { Width = 200, Margin = new Thickness(5) };
                        this.wholeComponentName.Tag = wholecompnameuid;
                        textBoxPanel.Children.Add(this.wholeComponentName);
                    } else {
                        this.proghmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+wholecompnameuid+" ParentUid:"+this.ParentUID+" OwnUID:"+this.OwnUID,-1082,4,4);
                    }
                    
                    this.connectionStatusText = new TextBlock();
                    textBoxPanel.Children.Add(this.connectionStatusText);
                    Grid.SetRow(textBoxPanel, 1);
                    grid.Children.Add(textBoxPanel);

                    // Asetetaan Canvas gridiin
                    Grid.SetRow(canvas, 2);
                    grid.Children.Add(canvas);

                    // Lisätään Grid ikkunaan
                    popupWindow.Content = grid;

                    // Näytetään ikkuna
                    popupWindow.Show();
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+retVal+" ParentUid:"+this.ParentUID+" OwnUID:"+this.OwnUID,-1080,4,4);
                    retVal=-3;
                }                    
            } else {
                this.proghmi.sendError(kutsuja+functionname,"ActionCentreUI class wasn't initialized! Errorcode:"+this.isclassOkay,-762,4,4);
                retVal=-2;
            }

            return retVal;
        }

        /// <summary> Yhteyden poisto metodi kahden laatikon väliltä </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="connection"> Connection, referenssi siihen connections listan Connection instanssiin, joka halutaan poistettavan </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään suorakulmio luodaan </param>
        /// <returns> {void} </returns>        
        private void DisconnectBoxes(string kutsuja, Connection connection, Canvas canvas)
        {
            string functionname="->(ACUI)DisconnectBoxes";

            long ownuid=connection.OwnUID;
            this.connhandler.RemoveSingleConnection(kutsuja+functionname,ownuid,canvas,this.actcenconhan.listofAllBoxesReferences); // Pitäisi poistaa yksi yhteys sekä tiedot yhteyden molempien päiden laatikoiden tiedoista

            /*
            int answ1=-1;
            int answ2=-1;

            long box1uid=connection.Box1OwnUID;
            long box2uid=connection.Box2OwnUID;

            ConnectionRectangle tempconrect;

            // Poistetaan ensin connectioneiden UID tiedot itse pikkulaatikoilta, jotka on yhdistetty toisiinsa
            if (this.listofAllBoxesReferences.IndexOfKey(box1uid)>-1) {
                tempconrect = this.listofAllBoxesReferences[box1uid];
                answ1=tempconrect.RemoveConnectionUID(kutsuja+functionname,ownuid);
                if (answ1<1) {
                    this.proghmi.sendError(kutsuja+functionname,"Removing object wasn't in ConnectionsUID list! Response:"+answ1+" RemovingUID:"+ownuid,-735,4,4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Unable to remove object uid from listofallboxes! Box1 UID:"+box1uid,-733,4,4);
            }

            if (this.listofAllBoxesReferences.IndexOfKey(box2uid)>-1) {
                tempconrect = this.listofAllBoxesReferences[box2uid];
                answ2=tempconrect.RemoveConnectionUID(kutsuja+functionname,ownuid);
                if (answ2<1) {
                    this.proghmi.sendError(kutsuja+functionname,"Removing object wasn't in ConnectionsUID list! Response:"+answ2+" RemovingUID:"+ownuid,-736,4,4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Unable to remove object uid from listofallboxes! Box2 UID:"+box2uid,-734,4,4);
            }     

            this.connhandler.connections.Remove(ownuid); // Sitten poistetaan itse kohde Connections listalta sen key arvolla
            this.objindexerref.DeleteObjectFromIndexer(kutsuja+functionname,ownuid); // Sen jälkeen kohde poistetaan kaikkien objektien listalta
            canvas.Children.Remove(connection.ConnectionLine); // Ja lopuksi kohde poistetaan piirrettävältä ruudulta
            */
        }

        /// <summary> Yhteyden luonti metodi kahden laatikon välille </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="canvas"> Canvas, referenssi siihen canvasiin, jonka sisään yhdysviivat luodaan </param>
        /// <param name="box1"> Rectangle, referenssi siihen suorakulmioon, joka halutaan yhdistää </param>
        /// <param name="box2"> Rectangle, referenssi siihen suorakulmioon, johon halutaan yhdistää </param>
        /// <param name="connectioncolorasbrushstring"> string, Brush muotoisen objektin väri muunnettuna string muotoiseksi. Jos tyhjä, niin käytetään this.selectedColor väriä </param>
        /// <returns> {int} Palauttaa 1, jos onnistui luomaan kohteen. Jos luku pienempi kuin 0, niin virhe. </returns>
        private int ConnectBoxes(string kutsuja, Canvas canvas, Rectangle box1, Rectangle box2, string connectioncolorasbrushstring="")
        {
            string functionname="->(ACUI)ConnectBoxes";
            Brush brush;
            int retVal=-6;

            if (connectioncolorasbrushstring!="") {
                try {
                    brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(connectioncolorasbrushstring));
                } catch (Exception e) {
                    // Käsittely virheelliselle värikoodille. Voit asettaa oletusvärin tai kirjoittaa virhelokin.
                    brush = this.selectedColor;  // Oletusväri
                    this.proghmi.sendError(kutsuja+functionname,"Colorcode error: " + e.Message,-985,4,4);
                }
            } else {
                brush=this.selectedColor;
            }

            long box1uid=(long)box1.Tag;
            long box2uid=(long)box2.Tag;

            if (box1uid>=0 && box2uid>=0) {
                retVal=this.connhandler.AddSingleConnection(kutsuja+functionname, canvas, box1uid, box2uid, this.actcenconhan.listofAllBoxesReferences, brush);            
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Error retvieve UI components UID:s. Box1uid:"+box1uid+" Box2uid:"+box2uid,-1052,4,4);
                retVal=-7;
            }
            return retVal;
        }
   
    }

    /// <summary>
    /// Tämä luokka pitää sisällään tarvittavat tiedot ParseJSON objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja
    /// </summary>
    public class JsonParsingStruct 
    {
        /// <summary>
        /// JSON parser luokan instanssi, joka hoitaa JSON objektien lukemisen
        /// </summary>
        public ParseJSON Parsejson;

        private long parserunninguid=0;
        /// <summary>
        /// Yksilöllinen referenssiarvo, jolla parserilla voidaan tarkistaa, että käännös on sama kuin lähetetty tieto
        /// </summary>
        public long Parserunninguid {
            get { return this.parserunninguid; }
            set { this.parserunninguid=value; }
        }

        /// <summary>
        /// Constructor JsonParsingStruct luokalle, joka pitää sisällään tarvittavat tiedot ParseJson objektista ja siihen liitetystä muuttujasta, joka antaa yksilöllisiä tunnustietoja
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="progh"> ProgramHMI, Käyttöliittymän referenssi </param>
        /// <param name="usenewparseobjects">bool, luodaanko aina uusi parseobjekti, jos edellistä ei ole vielä suljettu </param>
        /// <param name="debugarea">int, mitä areaa käytetään tietojen printtaamiseksi - esim. 4 on error area, 2 on alert area jne.</param>
        /// <returns> {void} </returns>
        public JsonParsingStruct(string kutsuja, ProgramHMI progh, bool usenewparseobjects=true, int debugarea=4)
        {
            this.Parsejson = new ParseJSON(progh,usenewparseobjects,debugarea);
        }
    }