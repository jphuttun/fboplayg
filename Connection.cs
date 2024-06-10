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
    
    /// <summary> Tämä luokka pitää tietonaan mitä yhdysviivoja ja yhdysviivoihin liittyviä graafisia komponentteja eri objektien välille on luotu </summary>
    public class Connection
    {
        private long box1motherboxuid;
        /// <summary> Laatikon 1. vanhemman uniqrefnum (eli sen laatikon UID jota voidaan vetää ruudulla toiseen kohtaan?? ehkä??) </summary>
        public long Box1MotherBoxUID { 
            get { return this.box1motherboxuid; }
            set {
                this.box1motherboxuid=value;               
            }
        }

        private long box1ownuid;
        /// <summary> Laatikon 1. (laatikko josta tiedot lähtevät) uniqrefnum </summary>
        public long Box1OwnUID { 
            get { return this.box1ownuid; }
            set {
                this.box1ownuid=value;
                if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1 && this.connuicomps!=null) {
                    this.connuicomps.Box1.Tag=value;
                }                 
            } 
        }

        private long box2motherboxuid;
        /// <summary> Laatikon 2. vanhemman uniqrefnum (eli sen laatikon UID jota voidaan vetää ruudulla toiseen kohtaan?? ehkä??) </summary>
        public long Box2MotherBoxUID { 
            get { return this.box2motherboxuid; }
            set {
                this.box2motherboxuid=value;                
            }
        }

        private long box2ownuid;
        /// <summary> Laatikon 2. (laatikko johon tiedot saapuvat) uniqrefnum </summary>
        public long Box2OwnUID { 
            get { return this.box2ownuid; }
            set {
                this.box2ownuid=value;
                if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1 && this.connuicomps!=null) {
                    this.connuicomps.Box2.Tag=value;
                }                 
            }
        } 

        private string infotextstring="";
        /// <summary>
        /// Yhdysviivan puoleen väliin tuleva teksti, johon voi printata haluamiaan tietoja - tässä on pelkästään teksti, ei itse graafista komponenttia
        /// </summary>
        public string InfoTextString { 
            get { return infotextstring; } 
            set {
                infotextstring=value;
                if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1 && this.connuicomps!=null) {
                    this.connuicomps.InfoText.Text=value;
                }
            } 
        }

        /// <summary> Yksittäisen connectionin oma uniqrefnum eli UID </summary>
        public long OwnUID { get; set; }

        /// <summary>
        /// Tämän ConnectionHandlerin vanhemman UID
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Tämän ConnectionHandlerin isovanhemman UID
        /// </summary>
        public long GranParentUID { get; set; }

        /// <summary>
        /// Käyttöliittymäluokan referenssi
        /// </summary> 
        private ProgramHMI proghmi;

        /// <summary>
        /// ObjectIndexer luokan referenssi - kyseinen luokka pitää ylhäällä, mitä objekteja olemme luoneet ohjelmaan, joiden ID:t täytyy olla rekisteröitynä
        /// </summary>
        private ObjectIndexer objIndexer;

        /// <summary> Tämän luokan referenssi pitää sisällään käyttöliittymän komponenttien tiedot connection viivojen välillä </summary>
        private ConnectionUIComponents connuicomps;
        /// <summary>
        /// Palauttaa aliluokan joka sisältää graafiset komponentit, jos sellainen aliluokkan instanssi on luotu
        /// </summary>
        public ConnectionUIComponents ReturnConnectionUIComponents {
            get {
                if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                    return this.connuicomps;
                } else {
                    this.proghmi.sendError("->(CON)ReturnConnectionUIComponents","UI components not created! Response:"+this.createuicomps+" OwnUID: "+this.OwnUID,-1042,4,4);
                    return null;
                }
            }
        }

        /// <summary>
        /// Luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista
        /// </summary>
        private int createuicomps=0;

        /// <summary>
        /// Tämä BlockAtomValue saa jossain vaiheessa tiedon, jonka tämä Connection tarjoilee eteenpäin muille OperationBlock tyyppisille blokeille
        /// </summary> 
        private BlockAtomValue sendingatomvalue;

        /// <summary>
        /// Tämä BlockAtomValue saa jossain vaiheessa tiedon, jonka tämä Connection tarjoilee eteenpäin muille OperationBlock tyyppisille blokeille
        /// </summary> 
        public BlockAtomValue ReturnSendingBlockAtomValueRef {
            get { return this.sendingatomvalue; }
        }

        /// <summary>
        /// Constructor - Luo instanssin joka pitää sisällään yhden - kahden laatikon välisen yhdysviivan ja sitä koskevat graafiset komponentit sekä tietokentät
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="prhmi"> ProgramHMI, käyttöliittymäluokan referenssi </param>
        /// <param name="objinde"> ObjectIndexer luokan referenssi - kyseinen luokka pitää ylhäällä, mitä objekteja olemme luoneet ohjelmaan, joiden ID:t täytyy olla rekisteröitynä </param>
        /// <param name="thisconnectionownUID"> long, Yksittäisen connectionin oma uniqrefnum </param>
        /// <param name="parentuid"> long, Tämän ConnectionHandlerin vanhemman UID </param>
        /// <param name="granparentuid"> long, Tämän ConnectionHandlerin isovanhemman UID </param>
        /// <param name="box1ownuid"> long, Laatikon 1. (laatikko josta tiedot lähtevät) uniqrefnum </param>
        /// <param name="box1parentuid"> long, Laatikon 1. vanhemman uniqrefnum (eli sen laatikon UID jota voidaan vetää ruudulla toiseen kohtaan?? ehkä??) </param>
        /// <param name="box2ownuid"> long, Laatikon 2. (laatikko johon tiedot saapuvat) uniqrefnum </param>
        /// <param name="box2parentuid"> long, Laatikon 2. vanhemman uniqrefnum (eli sen laatikon UID jota voidaan vetää ruudulla toiseen kohtaan?? ehkä??) </param>
        /// <param name="creategraphcomp"> int, luodaanko tämän luokan luonnin yhteydessä graafiset komponentit. 0=ei luoda, 1=luodaan. Katso kaikki vaihtoehdot createUIComponents enumeroinnista </param>
        /// <returns> {void} </returns>
        public Connection(string kutsuja, ProgramHMI prhmi, ObjectIndexer objinde, long thisconnectionownUID, long parentuid, long granparentuid, long box1ownuid, long box1parentuid, long box2ownuid, long box2parentuid, int creategraphcomp=(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1)
        {
            string functionname="->(CON)Connection";
            this.createuicomps=creategraphcomp;
            this.proghmi=prhmi;
            this.objIndexer=objinde;
            this.OwnUID=thisconnectionownUID;
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.Box1OwnUID=box1ownuid;
            this.Box1MotherBoxUID=box1parentuid;
            this.Box2OwnUID=box2ownuid;
            this.Box2MotherBoxUID=box2parentuid;
            this.sendingatomvalue=new BlockAtomValue(); // Luodaan BlockAtomValue jolla siirretään blokin "result" tietoa eteenpäin

            if (this.objIndexer.objectlist.IndexOfKey(this.OwnUID)>-1) {
                int resp=this.objIndexer.SetObjectToIndexerWithErrorReport(kutsuja+functionname,this.OwnUID,this);
                if (resp<0) {
                    this.proghmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! Unsuccesful object set! UID:"+this.OwnUID+" Response:"+resp,-1130,4,4); 
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! No entry with such key! UID:"+this.OwnUID,-1129,4,4);
            }            

            if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                long connuiuid=this.objIndexer.AddObjectToIndexer(kutsuja+functionname,this.OwnUID,(int)ObjectIndexer.indexerObjectTypes.CONNECTION_LINE_UI_COMPONENTS_112,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,this.ParentUID); // Tässä -1 tarkoittaa, että objarraytype tietoa ei ole määritelty
                if (connuiuid>=0) {
                    this.connuicomps=new ConnectionUIComponents(kutsuja+functionname,this.proghmi,this.objIndexer,this.OwnUID,connuiuid,this.ParentUID);
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+connuiuid+" Box1uid:"+box1ownuid+" Box2uid:"+box2ownuid,-1056,4,4);
                }
            } else {
                this.connuicomps=null;
            }
        }

        /// <summary>
        /// Tämä metodi poistaa yhteyden kohteet canvasista silloin kun ollaan tuhoamassa itse objektia. Sen lisäksi tämä metodi poistaa objectindexeristä yhteydelle rekisteröidyt objektit
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="origcanvas">Canvas, se canvas josta kyseiset graafiset komponentit poistetaan</param>
        /// <returns> {void} </returns>
        public void RemoveConnectionComponents(string kutsuja, Canvas origcanvas)
        {
            string functionname="->(CON)RemoveConnectionComponents";
            int answ4=-1;

            if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                if (this.connuicomps!=null) {
                    this.connuicomps.RemoveConnectionUIComponents(kutsuja+functionname,origcanvas); // Poistaa UI:n graafiset komponentit
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Error during delete UI components! ConnUIComps was null! Box1Uid:"+this.Box1OwnUID+" ParentUID: "+this.OwnUID+" OwnUID:"+this.OwnUID+" Box2Uid:"+this.Box2OwnUID,-1045,4,4);    
                }
            }

            // Poistetaan tämä oma objekti objectIndexeristä
            answ4=this.objIndexer.DeleteObjectFromIndexer(kutsuja+functionname,this.OwnUID);
            if (answ4<1) {
                this.proghmi.sendError(kutsuja+functionname,"Error during delete operation in ObjectIndexer! Response:"+answ4+" Box1Uid:"+this.Box1OwnUID+" ParentUID: "+this.OwnUID+" OwnUID:"+this.OwnUID+" Box2Uid:"+this.Box2OwnUID,-1044,4,4);
            }            
        }

        /// <summary> Yhteyden luonti metodi kahden laatikon välille </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="connectionuid"> long, yhteyden oma uid </param>
        /// <param name="connectiontextuid"> long, yhteyden graafiseen komponenttiin tulevan tekstin uid </param>
        /// <param name="origcanvas"> Canvas, referenssi siihen canvasiin, jonka sisään yhdysviivat luodaan </param>
        /// <param name="box1uid"> long, referenssi siihen suorakulmioon uid:iin, joka halutaan yhdistää </param>
        /// <param name="box2uid"> long, referenssi siihen suorakulmioon uid:iin, johon halutaan yhdistää </param>
        /// <param name="listofallboxesreferences">SortedList long, ConnectionRectangle, referenssi listaan, joka pitää kirjaa kaikista suorakulmioista jotka canvasille on piirretty </param>
        /// <param name="selectedcolor"> Brush, väri jolla viiva ja siihen liittyvä päätepiste halutaan piirtää </param>
        /// <returns> {int} Palauttaa 1, jos yhteyden luonti onnistui - muussa tapauksessa negatiivisen luvun virheenä. Jos -1=epämääräinen virhe, -2=virhe box1 laatikon referenssin noudossa, -3=virhe box2 laatikon referenssin noudossa ja -4=väärin intialisoitu luokka </returns>        
        public int AddConnection(string kutsuja, long connectionuid, long connectiontextuid, Canvas origcanvas, long box1uid, long box2uid, SortedList<long, ConnectionRectangle> listofallboxesreferences, Brush selectedcolor)
        {
            string functionname = "->(CON)AddSingleConnection";
            int retVal=-1;

            if (this.createuicomps==(int)ImportantProgramParams.createUIComponents.CREATE_UI_COMPONENTS_1) {
                if (this.connuicomps!=null) {
                    retVal=this.connuicomps.AddSingleConnectionUIComponents(kutsuja+functionname,connectionuid, connectiontextuid, origcanvas,box1uid,box2uid,listofallboxesreferences,selectedcolor);
                    if (retVal<1) {
                        this.proghmi.sendError(kutsuja+functionname,"Error during create UI components! Response:"+retVal,-1051,4,4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Error! UI components null reference! Box1UID:"+box1uid+" Box2UID:"+box2uid,-1084,4,4);
                    retVal=-19;
                }
            }
            return retVal;
        }        
    }

    /// <summary>
    /// Tämä luokka säilyttää yksittäisen Connectionin graafiset komponentit erillään tietomallista
    /// </summary>
    public class ConnectionUIComponents
    {
        /// <summary>
        /// Tämän ConnectionUIComponents instanssin isovanhemman UID
        /// </summary>
        public long GranParentUID { get; set; }

        /// <summary> Connection luokan instanssin oma UID, jonka alta tämä instanssi löytyy </summary>
        public long ParentUID { get; set; }

        /// <summary> Tämä tiedonsäilytysinstanssin oma UID </summary>
        public long OwnUID { get; set; }

        /// <summary> Sen laatikon graafisen komponentin referenssi, josta tiedot lähtevät yhdistysviivaa pitkin. Eli tiedot kulkevat aina laatikosta 1. laatikkoon 2. </summary>
        public Rectangle Box1 { get; set; }

        /// <summary> Sen laatikon graafisen komponentin referenssi, johon tiedot päätyvät. Eli tiedot kulkevat aina laatikosta 1. laatikkoon 2. </summary>
        public Rectangle Box2 { get; set; }

        /// <summary> Itse yhdysviivan graafinen komponentti joka piirretään laatikon 1. ja laatikon 2. välille </summary>
        public Line ConnectionLine { get; set; }

        /// <summary> Yhdysviivan lopetuspisteeseen piirrettävä ympyrä </summary>        
        public Ellipse EndPointCircle { get; set; }

        /// <summary> Yhdysviivan puoleen väliin tuleva teksti, johon voi printata haluamiaan tietoja </summary>
        public TextBlock InfoText { get; set; }

        /// <summary> Tämä muuttuja seuraa, onko luokka initialisoitu oikein </summary>
        private int isclassokay=-1;

        /// <summary>
        /// Tämä muuttuja kertoo, kuinka monta vaihetta initialisointia on käytävä läpi, ennenkuin luokka on oikealla tavalla initialisoitu
        /// </summary>
        private int classokaytreshold=1;

        /// <summary>
        /// Käyttöliittymän referenssi
        /// </summary>
        private ProgramHMI proghmi;

        /// <summary>
        /// Määrittelee, kuinka paljon tekstiä nostetaan ylemmäksi viivasta
        /// </summary>
        private double shiftTextToUp=12;

        /// <summary>
        /// ObjectIndexer luokan referenssi - kyseinen luokka pitää ylhäällä, mitä objekteja olemme luoneet ohjelmaan, joiden ID:t täytyy olla rekisteröitynä
        /// </summary>
        private ObjectIndexer objIndexer;

        /// <summary>
        /// Tämä property kertoo, onko luokka initialisoitu oikealla tavalla
        /// </summary>
        public bool IsClassInitialized {
            get {
                if (this.isclassokay>=this.classokaytreshold) {
                    return true;
                } else {
                    return false;
                }
            }
        }        

        /// <summary>
        /// Yhteen Connectioniin liittyvien graafisten komponenttien luokan constructor
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="promgh"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="objinde"> ObjectIndexer luokan referenssi. Kyseinen luokka huolehtii objektien rekisteröimisestä kohteille sekä antaa niille UID tiedot </param>
        /// <param name="parentuid">long, Connection luokan instanssin oma UID, jonka alta tämä instanssi löytyy</param>
        /// <param name="ownuid">long, tämä connectionin graafisten komponenttien säilytysluokan instanssin oma UID</param>
        /// <param name="granparentuid"> long, Tämän ConnectionUIComponents instanssin isovanhemman UID </param>
        /// <returns>{void}</returns>
        public ConnectionUIComponents(string kutsuja, ProgramHMI promgh, ObjectIndexer objinde, long parentuid, long ownuid, long granparentuid)
        {
            string functionname="->(CUIC)ConnectionUIComponents";
            this.objIndexer=objinde;
            this.proghmi=promgh;
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.OwnUID=ownuid;

            if (objinde.objectlist.IndexOfKey(this.OwnUID)>-1) {
                int resp=objinde.SetObjectToIndexerWithErrorReport(kutsuja+functionname,this.OwnUID,this);
                if (resp>=0) {
                    this.isclassokay++;
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! Unsuccesful object set! UID:"+ownuid+" Response:"+resp,-1121,4,4); 
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Fatal Error! Error to set object to objectindexer objectlist! No entry with such key! UID:"+ownuid,-1120,4,4);
            }
        }

        /// <summary>
        /// Tätä metodia tulee kutsua, jotta yksi ConnectionUIComponent tulee viimeisteltyä loppuun saakka
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="box1"> Rectangle, Sen laatikon graafisen komponentin referenssi, josta tiedot lähtevät yhdistysviivaa pitkin. Eli tiedot kulkevat aina laatikosta 1. laatikkoon 2. </param>
        /// <param name="box2"> Rectangle, Sen laatikon graafisen komponentin referenssi, johon tiedot päätyvät. Eli tiedot kulkevat aina laatikosta 1. laatikkoon 2. </param>
        /// <param name="connline"> Line, Itse yhdysviivan graafinen komponentti joka piirretään laatikon 1. ja laatikon 2. välille </param>
        /// <param name="endpointcircle"> Ellipse, Yhdysviivan lopetuspisteeseen piirrettävä ympyrä </param>
        /// <param name="infotext"> TextBlock, Yhdysviivan puoleen väliin tuleva teksti, johon voi printata haluamiaan tietoja </param>
        /// <returns> {void} </returns>
        public void InitializeConnectionUIComponents(string kutsuja, Rectangle box1, Rectangle box2, Line connline, Ellipse endpointcircle, TextBlock infotext)
        {
            this.Box1=box1;            
            this.Box2=box2;            
            this.ConnectionLine=connline;
            this.EndPointCircle=endpointcircle;
            this.InfoText=infotext;

            this.isclassokay++;
        }

        /// <summary> Tämä metodi päivittää yhden viivan sijainnin kahden yhdistämislaatikon väliltä ja siihen liittyvän tekstin paikan </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {int} Palauttaa 1, jos päivitys onnistui. Jos pienempi kuin 0, niin virhe. -1=määrittelemätön virhe, -2=virhe luokan initialisoinnissa </returns>
        public int UpdateConnectionOnCanvas(string kutsuja)
        {
            string functionname="->(CUIC)UpdateConnectionOnCanvas";
            int retVal=-1;

            if (this.isclassokay>=this.classokaytreshold) {
                // Get the center of the rectangles
                double x1 = Canvas.GetLeft(this.Box1) + this.Box1.Width / 2;
                double y1 = Canvas.GetTop(this.Box1) + this.Box1.Height / 2;
                double x2 = Canvas.GetLeft(this.Box2) + this.Box2.Width / 2;
                double y2 = Canvas.GetTop(this.Box2) + this.Box2.Height / 2;
                
                this.ConnectionLine.X1 = x1;
                this.ConnectionLine.Y1 = y1;
                this.ConnectionLine.X2 = x2;
                this.ConnectionLine.Y2 = y2;

                // Päivitetään loppupisteen ympyrä käyttöliittymässä
                Canvas.SetLeft(this.EndPointCircle, x2 - this.EndPointCircle.Width / 2);
                Canvas.SetTop(this.EndPointCircle, y2 - this.EndPointCircle.Height / 2);

                double midX = (x1 + x2) / 2;
                double midY = (y1 + y2) / 2;

                // Lasketaan kulma radiaaneina
                double angleRad = Math.Atan2(y2 - y1, x2 - x1);

                // Muunnetaan radiaanit asteiksi
                double angleDeg = angleRad * (180.0 / Math.PI);            

                // Kierretään tekstiä viivan kulman mukaan
                this.InfoText.RenderTransform = new RotateTransform(angleDeg, 0, 0);
                this.InfoText.RenderTransformOrigin = new Point(0.5, 0.5);            

                Canvas.SetLeft(this.InfoText, midX);
                Canvas.SetTop(this.InfoText, midY - shiftTextToUp); // Siirretään tekstiä ylöspäin, jotta se ei peitä viivaa

                retVal=1;

            } else {
                this.proghmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassokay,-1043,4,4);
                retVal=-2;
            }
            return retVal;
        }

        /// <summary> Yhteyden luonti metodi kahden laatikon välille </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="connectionuid"> long, yhteyden oma uid </param>
        /// <param name="connectiontextuid"> long, yhteyden graafiseen komponenttiin tulevan tekstin uid </param>
        /// <param name="origcanvas"> Canvas, referenssi siihen canvasiin, jonka sisään yhdysviivat luodaan </param>
        /// <param name="box1uid"> long, referenssi uid siihen suorakulmioon, joka halutaan yhdistää </param>
        /// <param name="box2uid"> long, referenssi uid siihen suorakulmioon, johon halutaan yhdistää </param>
        /// <param name="listofallboxesreferences">SortedList long, ConnectionRectangle, referenssi listaan, joka pitää kirjaa kaikista suorakulmioista jotka canvasille on piirretty </param>
        /// <param name="selectedcolor"> Brush, väri jolla viiva ja siihen liittyvä päätepiste halutaan piirtää </param>
        /// <returns> {int} Palauttaa 1, jos komponenttien luonti onnistui. Jos -1=epämääräinen virhe, -2=virhe box1 laatikon referenssin noudossa, -3=virhe box2 laatikon referenssin noudossa ja -4=väärin intialisoitu luokka </returns>        
        public int AddSingleConnectionUIComponents(string kutsuja, long connectionuid, long connectiontextuid, Canvas origcanvas, long box1uid, long box2uid, SortedList<long, ConnectionRectangle> listofallboxesreferences, Brush selectedcolor)
        {
            string functionname = "->(CUIC)AddSingleConnection";
            int retVal=-1;

            if (this.IsClassInitialized==true) {

                if (listofallboxesreferences.IndexOfKey(box1uid)>-1) {

                    if (listofallboxesreferences.IndexOfKey(box2uid)>-1) {
                        ConnectionRectangle tempconnectionrect;

                        Rectangle box1=listofallboxesreferences[box1uid].RectangleObject;
                        Rectangle box2=listofallboxesreferences[box2uid].RectangleObject;

                        double x1 = Canvas.GetLeft(box1) + box1.Width / 2;
                        double y1 = Canvas.GetTop(box1) + box1.Height / 2;
                        double x2 = Canvas.GetLeft(box2) + box2.Width / 2;
                        double y2 = Canvas.GetTop(box2) + box2.Height / 2;

                        Line line = new Line
                        {
                            X1 = x1,
                            Y1 = y1,
                            X2 = x2,
                            Y2 = y2,
                            Stroke = selectedcolor, 
                            StrokeThickness = 2
                        };
                        origcanvas.Children.Add(line);

                        // Lasketaan viivan keskipiste
                        double midX = (x1 + x2) / 2;
                        double midY = (y1 + y2) / 2;

                        // Lasketaan kulma radiaaneina
                        double angleRad = Math.Atan2(y2 - y1, x2 - x1);

                        // Muunnetaan radiaanit asteiksi
                        double angleDeg = angleRad * (180.0 / Math.PI);            

                        // Luodaan teksti "None" viivan keskelle
                        TextBlock newtext = new TextBlock
                        {
                            Text = "None",
                            Foreground = selectedcolor
                        };

                        // Kierretään tekstiä viivan kulman mukaan
                        newtext.RenderTransform = new RotateTransform(angleDeg, 0, 0);
                        newtext.RenderTransformOrigin = new Point(0.5, 0.5);

                        Canvas.SetLeft(newtext, midX);
                        Canvas.SetTop(newtext, midY - shiftTextToUp); // Siirretään tekstiä ylöspäin, jotta se ei peitä viivaa
                        origcanvas.Children.Add(newtext);            

                        Ellipse endpointcircle = new Ellipse
                        {
                            Width = 7,
                            Height = 7,
                            Fill = line.Stroke
                        };

                        // Päivitetään loppupisteen ympyrä käyttöliittymässä
                        Canvas.SetLeft(endpointcircle, x2 - endpointcircle.Width / 2);
                        Canvas.SetTop(endpointcircle, y2 - endpointcircle.Height / 2);

                        origcanvas.Children.Add(endpointcircle);                    
                        
                        line.Tag=connectionuid; // Asetetaan yhdysviivaan ja sen päätypisteympyrään Connectionin UID Tag:iin tiedoksi, sillä objektit löytyvät, jos tietää mikä UID on kyseessä
                        endpointcircle.Tag=connectionuid;

                        newtext.Tag=connectiontextuid;

                        this.InitializeConnectionUIComponents(kutsuja+functionname,box1,box2,line, endpointcircle, newtext); // Asetetaan graafiset komponentit

                        retVal=1;
                    } else {
                        retVal=-2;
                        this.proghmi.sendError(kutsuja+functionname,"Error during reading listofallboxesreferences! Error:"+retVal+" Box2uid:"+box2uid+" Response is -1",-1049,4,4);
                    }
                } else {
                    retVal=-3;
                    this.proghmi.sendError(kutsuja+functionname,"Error during reading listofallboxesreferences! Error:"+retVal+" Box1uid:"+box1uid+" Response is -1",-1050,4,4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Class not initialized correctly! Initialization number:"+this.isclassokay,-1048,4,4);
                retVal=-9;
            }
            return retVal;            
        }            

        /// <summary>
        /// Tämä metodi poistaa yhteyden kohteet canvasista silloin kun ollaan tuhoamassa itse objektia. Sen lisäksi tämä metodi poistaa objectindexeristä yhteydelle rekisteröidyt objektit
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="origcanvas">Canvas, se canvas josta kyseiset komponentit poistetaan</param>
        /// <returns> {void} </returns>
        public void RemoveConnectionUIComponents(string kutsuja, Canvas origcanvas)
        {
            string functionname="->(CUIC)RemoveConnectionUIComponents";
            int answ4=-1;
            int answ3=-1;

            long infotextuid=(long)this.InfoText.Tag; // Otetaan InfoText Textblockin uid talteen

            // Poistetaan viivan yläpuolelle tulevan tekstin objekti objectindexeristä
            answ4=this.objIndexer.DeleteObjectFromIndexer(kutsuja+functionname,infotextuid);
            if (answ4<1) {
                this.proghmi.sendError(kutsuja+functionname,"Error during delete operation in ObjectIndexer! Response:"+answ4+" Box1Uid:"+(long)this.Box1.Tag+" ParentUID: "+this.OwnUID+" OwnUID:"+infotextuid+" Box2Uid:"+(long)this.Box2.Tag,-768,4,4);    
            }

            answ3=this.objIndexer.DeleteObjectFromIndexer(kutsuja+functionname,this.OwnUID); // Sen jälkeen kohde poistetaan kaikkien objektien listalta
            if (answ3<1) {
                this.proghmi.sendError(kutsuja+functionname,"Error during delete operation in ObjectIndexer! Response:"+answ3+" Box1Uid:"+(long)this.Box1.Tag+" OwnUID:"+this.OwnUID+" Box2Uid:"+(long)this.Box2.Tag,-748,4,4);
            }            

            if (origcanvas!=null) {
                // Poistetaan viiva
                if (ConnectionLine!=null) {
                    origcanvas.Children.Remove(this.ConnectionLine);
                    this.ConnectionLine=null;
                }
                // Poistetaan loppupisteen ympyrä
                if (EndPointCircle != null) 
                {
                    origcanvas.Children.Remove(this.EndPointCircle);
                    this.EndPointCircle = null;
                }
                // Poistetaan Infoteksti
                if (InfoText != null) {
                    origcanvas.Children.Remove(this.InfoText);
                    this.InfoText = null;
                }
                // Poistetaan alkulaatikko
                if (Box1!=null) {
                    this.Box1=null;
                }
                // Poistetaan loppulaatikko
                if (Box2!=null) {
                    this.Box2=null;
                }
            }           
        }               
    }    
