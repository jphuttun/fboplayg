using System;
using System.Collections.Generic; // List toiminto löytyy tämän sisästä
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
    
    /// <summary> Tämä luokka pitää sisällään yhden slotin tiedot. Slotti kuvaa ohjelmani osakekaupankäynnissä geneeristä aluetta, jonka sisällä kaupankäyntiä koskee sama kokoelma käyttäjän määrittämiä sääntöjä. Slotti voi olla esimerkiksi jokin
    /// kurssialue, esim. vaikka 90 $ - 100 $ välinen kurssialue (toteutettu ominaisuus tällä hetkellä) tai vaikka kurssia seuraavan trendin kerroin k=0,8 - 1,0 (ei vielä toteutettu ominaisuus tällä hetkellä). </summary>
    public class OneSlot
    {
        /// <summary> Tämä muuttuja pitää sisällään sen moduulin nimen, jotka ladataan ActionCentreUI moduuliin ja jota käytetään tämän slotin blokkien ajossa. 
        /// Jos muuttuja on tyhjä, käytetään slottilistan blokkirakennetta, muussa tapauksessa tämän slotin omaa blokkirakennetta. Näin pystytään asettamaan jopa yksittäisille sloteille slottilistasta poikkeavia blokki patterneja </summary>
        private string usedactioncentrepatternname="";
        /// <summary> Tämä muuttuja pitää sisällään sen moduulin nimen, jotka ladataan ActionCentreUI moduuliin ja jota käytetään tämän slotin blokkien ajossa. 
        /// Jos muuttuja on tyhjä, käytetään slottilistan blokkirakennetta, muussa tapauksessa tämän slotin omaa blokkirakennetta. Näin pystytään asettamaan jopa yksittäisille sloteille slottilistasta poikkeavia blokki patterneja </summary>
        public string UsedActionCentrePatternName {
            get { return this.usedactioncentrepatternname; }
            set { this.usedactioncentrepatternname=value; }
        }

        /// <summary> Temporary MarkerSignal, jota voidaan käyttää tietojen päivitysten syöttämisessä SaveMarkerController luokalle </summary>
        private long tempmarkersignal=-1;

        private long _objuniquerefnum;
        /// <summary> Pitää sisällään slotin referenssinumeron, jonka ObjectIndexer on sille antanut </summary>
        public long ObjUniqueRefNum { 
            get { return this._objuniquerefnum; } 
            set { 
                this._objuniquerefnum=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)ObjUniqueRefNum",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);
            }
        }

        /// <summary>
        /// Tämän objektin vanhemman UID tieto ObjectIndexerissä
        /// </summary>
        public long ParentUID { get; set; } 

        private decimal _slotvalue;
        /// <summary> Pitää sisällään sen kohdan, johon slotti luotiin </summary>
        public decimal SlotValue { 
            get { return this._slotvalue; } 
            set { 
                this._slotvalue=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)SlotValue",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        }

        private decimal _ordertruevalue;
        /// <summary> Jos syystä tai toisesta Orderin value on erilainen, kuin Slotin value, niin se kirjoitetaan tähän </summary>
        public decimal OrderTrueValue { 
            get { return this._ordertruevalue; } 
            set { 
                this._ordertruevalue=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)OrderTrueValue",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        private int _slotareap;
        /// <summary> Slottilistan areapointer, eli mihin listaan slotti kuuluu slotlistarrayssa </summary>
        public int SlotAreap { 
            get { return this._slotareap; } 
            set { 
                this._slotareap=value; 
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)SlotAreap",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        }

        private long _orderindex;
        /// <summary> Orderin indeksi numero orders listassa </summary>
        public long OrderIndex { 
            get { return this._orderindex; } 
            set { 
                this._orderindex=value; 
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)OrderIndex",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        private int _slotstatus;
        /// <summary> -1 = Ei koskaan käytetty, 0 = Käyttämätön, 1 = Slotissa osto orderi, 2 = Slotissa myyntiorderi </summary>
        public int SlotStatus { 
            get { return this._slotstatus; } 
            set { 
                this._slotstatus=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)SlotStatus",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                  
            } 
        } 

        private int _underoperation;
        /// <summary> Jos 1 = slottia ollaan paraikaa liittämässä osto Orderiin. Jos 2 = slottia ollaan muuntamassa myyntiorderiksi. Jos <1, slotti on vapaana toimimaan (0=vapaa toimintaan, -1=vapaa ja ei koskaan käytetty) </summary>
        public int UnderOperation { 
            get { return this._underoperation; } 
            set { 
                this._underoperation=value; 
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)UnderOperation",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        private int _soldbought;
        /// <summary> -1 = Ei koskaan käytetty, 0 = Käyttämätön, 1 = Slotin osto-orderi on toteutunut - voidaan edetä kohteen myyntiin, 2 = Slotin myyntiorderi on toteutunut, voidaan edetä slotin tyhjäämiseen ja siirtämään se moodiin käyttämätön </summary>
        public int SoldBought { 
            get { return this._soldbought; } 
            set { 
                this._soldbought=value; 
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)SoldBought",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        private bool _iscancelling;
        /// <summary> false=ei ole cancelointi tilassa, true = ollaan paraikaa canceloimassa slottia, eikä slottia saa työntää uudestaan sillä välin cancelling tilaan </summary>
        public bool IsCancelling { 
            get { return this._iscancelling; } 
            set { 
                this._iscancelling=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)IsCancelling",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        private decimal _slotsize;
        /// <summary> Slotin osto/myynti koko </summary>
        public decimal SlotSize { 
            get { return this._slotsize; } 
            set { 
                this._slotsize=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)SlotSize",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        /// <summary>
        /// Tämän luokan instanssi pitää sisällään AltRoute tiedot ja blokkien lukuun liittyvät yksityiskohdat sekä reattempt tapaukset
        /// </summary>
        public AttemptAndInstructionObject blockinstructobj; 

        /// <summary> Onko vaihtoehtoista reittiä osto/myynti tapahtumille (vastakkainen tuotto), -1=ei käytetty, 0=normaali proseduuri käynnissä, 1=kaksi ostopistettä luotu (ylös ja alas), 2=alempi myyntipiste toteutunut, 3=alempi triggerpiste alitettu, 4=alempi ostopiste luotu, 5=ylempi ostopiste luotu, joka on sama kuin SoldBought=2 HUOM! Nykyään paljon mahdollisia vapaasti määriteltäviä variaatioita!</summary>
        public int AltRoute { 
            get { 
                return this.blockinstructobj.GetSingleAltRoute("PRP-OneSlot-AltRoute"); 
            } 
            set { 
                this.blockinstructobj.SetSingleAltRoute("PRP-OneSlot-AltRoute",value);
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)AltRoute",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                  
            } 
        } 

        private bool _isconditionalorder;
        /// <summary> Onko kyseessä trigger order tyyppinen kohde vai ei </summary>
        public bool IsConditionalOrder { 
            get { return this._isconditionalorder; } 
            set { 
                this._isconditionalorder=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)IsConditionalOrder",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 
        
        private int orderattachmenttype=0;
        /// <summary> Onko Order liitetty slottiin vai ei - 0=Ei ole liitetty, 1=On liitetty, 2=Inner order attachment action type, joka tarkoittaa, että ei voida toteuttaa sen tyyppistä toimeksiantoa, joten toimeksianto joudutaan kirjaamaan ohjelman sisäiselle listalle ja toteuttamaan sieltä käsin, kun kytätty ajanhetki tulee eteen. </summary>
        public int IsOrderAttachedType { 
            get { return this.orderattachmenttype; } 
            set { 
                this.orderattachmenttype=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)IsOrderAttachedType",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        /// <summary> Muutetaan OrderAttachment boolean tyypiksi </summary>
        public bool IsOrderAttached
        {
            set {
                if (value==true) {
                    this.orderattachmenttype=1;
                } else {
                    this.orderattachmenttype=0;
                }
            }
            get {
                if (orderattachmenttype==0) {
                    return false;
                } else {
                    return true;
                }
            }
        }        

        /// <summary> long, Tämän ei ole tarkoitus muuttua sitten luontihetken. Slotille yksilöllinen luontijärjestysnumero, mutta ei kuitenkaan UID vaan slotin luonnin järjestysnumero joka kasvaa yhdellä joka kerta kun slottilistaan on luotu uusi kohde. Tällä numerolla kohde löytyy varsinaisesti slottilistasta, mutta tämä on eri numero, kuin millä kohde löytyy ObjectIndexeristä. </summary>
        private long slotcreationnumber=-1;
        /// <summary> long, Tämän ei ole tarkoitus muuttua sitten luontihetken. Slotille yksilöllinen luontijärjestysnumero, mutta ei kuitenkaan UID vaan slotin luonnin järjestysnumero joka kasvaa yhdellä joka kerta kun slottilistaan on luotu uusi kohde. Tällä numerolla kohde löytyy varsinaisesti slottilistasta, mutta tämä on eri numero, kuin millä kohde löytyy ObjectIndexeristä. </summary>
        public long SlotCreationNumber { 
            get { return this.slotcreationnumber; } 
        } 

        /// <summary> DeviceString, BotIndex ja Launchnumber yhdistettynä tuottaa slotlistpereid muuttujan ja siihen vielä lisätään SlotlistUniqueIndex viittaamaan mikä slotlistarray:n lista on kyseessä sekä SlotUniqueCrationNumber merkkaamaan slotin yksilöllisyyttä, josta saadaan SlotUniqueId aikaiseksi </summary>
        private string slotuniqid="";
        /// <summary> Slotin yksilöllinen id - DeviceString, BotIndex ja Launchnumber yhdistettynä tuottaa slotlistpereid muuttujan ja siihen vielä lisätään SlotlistUniqueIndex viittaamaan mikä slotlistarray:n lista on kyseessä sekä SlotUniqueCrationNumber merkkaamaan slotin yksilöllisyyttä, josta saadaan SlotUniqueId aikaiseksi </summary>
        public string SlotUniqueId { 
            get { return this.slotuniqid; } 
        }

        private long orderidnum=-1;       
        /// <summary> Kauppapaikan Orderin id numero, joka on liitetty slotin toimeksiantoon </summary>
        public long OrderIdNum { 
            get { return orderidnum; } 
            set { 
                orderidnum=value; 
                if (this.IsOrderAttachedType==0) this.IsOrderAttachedType=1; 
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)OrderIdNum",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        private long markercounter=-1;
        /// <summary> Slotin heartbeat counter. Käytetään käytännössä samaa counteria kuin signalcounter </summary>
        public long MarkerCounter { 
            get { return this.markercounter; }
            set { 
                this.markercounter=value;
                if (this.markercounteratcreation<0) { // Jos setataan markercounter ensimmäistä kertaa, otetaan aloitushetki talteen
                    this.markercounteratcreation=this.markercounter;
                } 
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)MarkerCounter",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            }
        }

        private long markercounteratcreation=-1;
        /// <summary> Slotin hearbeat counter tiettyä työvaihetta aloittaessa - tällä pyritään estämään saman actionin uudelleen ajamista, ennenkuin heartbeat on muuttunut aloitusarvosta toiseksi </summary>
        public long MarkerCounterAtCreation { 
            get { return this.markercounteratcreation; }
        }

        private decimal firsttriggervalue=-1;
        /// <summary> Kurssin arvo, jossa triggerikäytäntö toteutui ja alkoi käyttää AltRoute tietoa siitä, missä vaiheessa kyseinen slotti on toimenpidepolkua </summary>
        public decimal FirstTriggerValue { 
            get { return this.firsttriggervalue; } 
            set { 
                this.firsttriggervalue=value;
                this.IsConditionalOrder=true;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)FirstTriggerValue",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            }
        }

        private decimal uppertriggersellpoint=-1;
        /// <summary> FirstTriggerValue:n raja-arvon alituksen jälkeen määritellään ylempi myyntipiste, jossa STOP_LOSS tyylisesti pyritään myymään erä tappiollakin pois </summary>
        public decimal UpperTriggerSellPoint { 
            get { return this.uppertriggersellpoint; } 
            set {
                this.uppertriggersellpoint=value;
                this.IsConditionalOrder=true;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)UpperTriggerSellPoint",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            }
        }

        private decimal uppertriggersafetylimit=-1;
        /// <summary> Turvaraja UpperTriggerSellPointille, jossa kohde myydään market toimintona, mikäli kurssi on pompannut niin ylös, eikä jäädä odottelemaan esim. LIMIT tyyppistä myyntiä </summary>
        public decimal UpperTriggerSafetyLimit {
            get { return this.uppertriggersafetylimit; } 
            set {
                this.uppertriggersafetylimit=value;
                this.IsConditionalOrder=true;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)UpperTriggerSafetyLimit",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            }
        }

        private decimal lowertriggersellingpoint=-1;
        /// <summary> Alaraja FirstTriggerValuen jälkeen, jonka alapuolella kyseisellä myydyllä erällä voidaan tehdä edelleen kauppaa siinä vaiheessa kun erä on kytkeytynyt käyttöön </summary>
        public decimal LowerTriggerSellPoint { 
            get { return this.lowertriggersellingpoint; }
            set {
                this.lowertriggersellingpoint=value;
                this.IsConditionalOrder=true;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)LowerTriggerSellPoint",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            }
        } 

        private decimal premiotriggersellpoint=-1;
        /// <summary> Premio alaraja FirstTriggerValuen jälkeen, jonka alapuolella kyseisellä myydyllä erällä voidaan tehdä edelleen kauppaa siinä vaiheessa kun erä on kytkeytynyt käyttöön, mutta nyt kytätään premio myyntiä sitten tarvittaessa </summary>
        public decimal PremioTriggerSellPoint { 
            get { return this.premiotriggersellpoint; } 
            set {
                this.premiotriggersellpoint=value;
                this.IsConditionalOrder=true;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)PremioTriggerSellPoint",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            }
        }

        private int triggerreservedval=-1;
        public int TriggerReserved { 
            get { return this.triggerreservedval; } 
            set {
                this.triggerreservedval=value;
                this.IsConditionalOrder=true;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)TriggerReserved",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            }
        } 

        /// <summary> Jos yritetään saman slotin sisällä toimia useammin kuin kerran </summary>
        private int amountofextraattemps=0; 

        /// <summary> Kuinka monta attemptia/reattemptia voidaan tehdä yhden slotin alueella maksimissaan (tämä luku rajoittaa arrayn kokoa reattempteille, joten ei tätä lukua enempää jokatapauksessa) </summary>
        private int maxattempts=10;
        private int[] extraattemptsaltroutes;
        private decimal[] uppertriggersellingpoints; 
        private decimal[] firstriggervalues;
        private decimal[] lowertriggersellingpoints; 

        private long[] triggerorderindexes;

        private int[] triggerreserved;

        /// <summary> Objekti seller side sloteille assetteja varten </summary>
        public TriggerRefObject sellerobject; 
        
        private long _sellerobjuniquenum;
        /// <summary> (Seller) objektin uniikki numero, mutta jatkossa myös slotin uniikki numero </summary>
        public long SellerObjUniqueNum { 
            get { return this._sellerobjuniquenum; } 
            set { 
                this._sellerobjuniquenum=value;
                this.tempmarkersignal=this.smartbotikka.tradeordref.MarkerSingal;
                this._savemarkedctrl.AddSaveMarked("(OS)SellerObjUniqueNum",this._objuniquerefnum,this._slotvalue,this._slotareap,this.tempmarkersignal);                 
            } 
        } 

        /// <summary> Objekti buying side sloteille assetteja varten </summary>
        public NormalRefObject holdingobject; 

        /// <summary> Slotin luoneen SmartBotin referenssi slotille </summary>
        private SmartBot smartbotikka;

        /// <summary> SaveMarkerController luokan referenssi, jonne voidaan syöttää tieto muuttuneesta propertystä ja valvoa sitä kautta, tulisiko kohde tallentaa levylle </summary>
        private SaveMarkedController _savemarkedctrl;

        /// <summary> Käyttöliittymäluokan referenssi </summary>
        private ProgramHMI phmi;

        /// <summary> Lista kaikista parametrien nimistä MyOwnPrintSlots parametria varten </summary>
        public List<string> allParamNames;

        /// <summary> Apulista yksittäiselle parametrille </summary>
        private List<string> helperlist;

        /// <summary>
        /// ObjectIndexer luokan referenssi, joka jakaa UID numerot eri luotaville komponenteille ja sisältää tietoja, minkälaisesta komponentista on kyse
        /// </summary>
        private ObjectIndexer objindexer;

        /// <summary>
        /// Enumeraatio, joka sisältää kaikki slotin vakio propertyjen parametrit.
        /// </summary>
        public enum SlotParams {
            ObjUniqueRefNum = 10,
            SlotValue = 20,
            OrderTrueValue = 30,
            IsOrderAttachedType = 40,
            IsOrderAttached = 50,
            SlotCreationNumber = 60,
            SlotUniqueId = 70,
            SlotAreap = 80,
            OrderIdNum = 90,
            MarkerCounter = 100,
            MarkerCounterAtCreation = 110,
            OrderIndex = 120,
            SlotStatus = 130,
            UnderOperation = 140,
            SoldBought = 150,
            IsCancelling = 160,
            SlotSize = 170,
            AltRoute = 180,
            FirstTriggerValue = 190,
            UpperTriggerSellPoint = 200,
            UpperTriggerSafetyLimit = 210,
            LowerTriggerSellPoint = 220,
            PremioTriggerSellPoint = 230,
            TriggerReserved = 240,
            IsConditionalOrder = 250,
            SellerObjUniqueNum = 260
        }

        /// <summary>
        /// Metodi, joka asettaa OneSlot objektista blockparamvaluetype enumeraatiokohtaa vastaavan tiedon blockatom:in samantyyppiseen tietosäiliöön
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="blockatom"> BlockAtomValue, luokan instanssin referenssi, jonne sijoitetaan asetettava arvo sitä vastaavaan kohtaan </param>
        /// <param name="blockparamvaluetype"> int, OneSlot.SlotParams enumeraatio tyyppiä vastaava int tieto, jonka perusteella osataan asettaa blockatomiin oikea parametri OneSlot luokan instanssista</param>
        /// <param name="oneslot">OneSlot luokan instanssin referenssi, josta voidaan asettaa oikea arvo BlockAtomValue instanssiin blockparamvaluetype tiedon perusteella</param>
        /// <returns>{int} Palauttaa positiivisena lukuna blockatomvalue:n tietotyypin numeron (1-5), jos toimenpide onnistui. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos toimenpide epäonnistui. </returns>
        public int SetSlotValueParamToBlockAtomValue(string kutsuja, BlockAtomValue blockatom, int blockparamvaluetype, OneSlot oneslot) {
            int retVal = -30; // Epämääräinen virhe
            string functionname = "->(VB)SetSlotValueParamToBlockAtomValue";

            if (blockatom == null) {
                this.phmi.sendError(kutsuja + functionname, "BlockAtomValue is null", -1209, 4, 4);
                return -31;
            }

            if (oneslot == null) {
                this.phmi.sendError(kutsuja + functionname, "OneSlot is null", -1210, 4, 4);
                return -32;
            }

            if (!Enum.IsDefined(typeof(OneSlot.SlotParams), blockparamvaluetype)) {
                this.phmi.sendError(kutsuja + functionname, "Invalid blockparamvaluetype", -1211, 4, 4);
                return -33;
            }

            try {
                OneSlot.SlotParams paramType = (OneSlot.SlotParams)blockparamvaluetype;
                switch (paramType) {
                    case OneSlot.SlotParams.ObjUniqueRefNum:
                        blockatom.LongAtom = oneslot.ObjUniqueRefNum;
                        retVal=2;
                        break;
                    case OneSlot.SlotParams.SlotValue:
                        blockatom.DecAtom = oneslot.SlotValue;
                        retVal=3;
                        break;
                    case OneSlot.SlotParams.OrderTrueValue:
                        blockatom.DecAtom = oneslot.OrderTrueValue;
                        retVal=3;
                        break;
                    case OneSlot.SlotParams.IsOrderAttachedType:
                        blockatom.IntAtom = oneslot.IsOrderAttachedType;
                        retVal=1;
                        break;
                    case OneSlot.SlotParams.IsOrderAttached:
                        blockatom.BoolAtom = oneslot.IsOrderAttached;
                        retVal=5;
                        break;
                    case OneSlot.SlotParams.SlotCreationNumber:
                        blockatom.LongAtom = oneslot.SlotCreationNumber;
                        retVal=2;
                        break;
                    case OneSlot.SlotParams.SlotUniqueId:
                        blockatom.StringAtom = oneslot.SlotUniqueId;
                        retVal=4;
                        break;
                    case OneSlot.SlotParams.SlotAreap:
                        blockatom.IntAtom = oneslot.SlotAreap;
                        retVal=1;
                        break;
                    case OneSlot.SlotParams.OrderIdNum:
                        blockatom.LongAtom = oneslot.OrderIdNum;
                        retVal=2;
                        break;
                    case OneSlot.SlotParams.MarkerCounter:
                        blockatom.LongAtom = oneslot.MarkerCounter;
                        retVal=2;
                        break;
                    case OneSlot.SlotParams.MarkerCounterAtCreation:
                        blockatom.LongAtom = oneslot.MarkerCounterAtCreation;
                        retVal=2;
                        break;
                    case OneSlot.SlotParams.OrderIndex:
                        blockatom.LongAtom = oneslot.OrderIndex;
                        retVal=2;
                        break;
                    case OneSlot.SlotParams.SlotStatus:
                        blockatom.IntAtom = oneslot.SlotStatus;
                        retVal=1;
                        break;
                    case OneSlot.SlotParams.UnderOperation:
                        blockatom.IntAtom = oneslot.UnderOperation;
                        retVal=1;
                        break;
                    case OneSlot.SlotParams.SoldBought:
                        blockatom.IntAtom = oneslot.SoldBought;
                        retVal=1;
                        break;
                    case OneSlot.SlotParams.IsCancelling:
                        blockatom.BoolAtom = oneslot.IsCancelling;
                        retVal=5;
                        break;
                    case OneSlot.SlotParams.SlotSize:
                        blockatom.DecAtom = oneslot.SlotSize;
                        retVal=3;
                        break;
                    case OneSlot.SlotParams.AltRoute:
                        blockatom.IntAtom = oneslot.AltRoute;
                        retVal=1;
                        break;
                    case OneSlot.SlotParams.FirstTriggerValue:
                        blockatom.DecAtom = oneslot.FirstTriggerValue;
                        retVal=3;
                        break;
                    case OneSlot.SlotParams.UpperTriggerSellPoint:
                        blockatom.DecAtom = oneslot.UpperTriggerSellPoint;
                        retVal=3;
                        break;
                    case OneSlot.SlotParams.UpperTriggerSafetyLimit:
                        blockatom.DecAtom = oneslot.UpperTriggerSafetyLimit;
                        retVal=3;
                        break;
                    case OneSlot.SlotParams.LowerTriggerSellPoint:
                        blockatom.DecAtom = oneslot.LowerTriggerSellPoint;
                        retVal=3;
                        break;
                    case OneSlot.SlotParams.PremioTriggerSellPoint:
                        blockatom.DecAtom = oneslot.PremioTriggerSellPoint;
                        retVal=3;
                        break;
                    case OneSlot.SlotParams.TriggerReserved:
                        blockatom.IntAtom = oneslot.TriggerReserved;
                        retVal=1;
                        break;
                    case OneSlot.SlotParams.IsConditionalOrder:
                        blockatom.BoolAtom = oneslot.IsConditionalOrder;
                        retVal=5;
                        break;
                    case OneSlot.SlotParams.SellerObjUniqueNum:
                        blockatom.LongAtom = oneslot.SellerObjUniqueNum;
                        retVal=2;
                        break;
                    default:
                        this.phmi.sendError(kutsuja + functionname, "Unsupported blockparamvaluetype", -1212, 4, 4);
                        return -34;
                }

            } catch (Exception ex) {
                this.phmi.sendError(kutsuja + functionname, "Exception: " + ex.Message, -1213, 4, 4);
                retVal = -35;
            }

            return retVal;
        }        

        /// <summary>
        /// Palauttaa enumeraationa (int) arvon annettuun parametriin perustuen.
        /// </summary>
        /// <param name="kutsuja">string, representing the caller's path.</param>
        /// <param name="paramName">string, Parametri, joka muunnetaan enumeraation arvoksi.</param>
        /// <returns>Enumeraation (int) arvo, jos parametri löytyy; muuten -1.</returns>
        public static int GetSlotParamValue(string kutsuja, string paramName) {
            if (Enum.TryParse(typeof(SlotParams), paramName, out var result)) {
                return (int)result;
            }
            return -1; // Palautetaan -1, jos parametri ei ole kelvollinen
        }

        /// <summary>
        /// Palauttaa parametrin nimen annettuun enumeraation (int) arvoon perustuen.
        /// </summary>
        /// <param name="kutsuja">string, representing the caller's path.</param>
        /// <param name="paramValue">int, Enumeraation (int) arvo, joka muunnetaan parametrin nimeksi.</param>
        /// <returns>Parametrin nimi, jos arvo löytyy; muuten null.</returns>
        public static string GetSlotParamName(string kutsuja, int paramValue) {
            return Enum.GetName(typeof(SlotParams), paramValue);
        }

        /// <summary>
        /// Constructor OneSlot objektille, joka säilyttää tarvittavia tietoja yhden toimintapisteen kohdasta järjestelmässä (slottilistassa)
        /// </summary>
        /// <param name="kutsuja">A string representing the caller's path.</param>
        /// <param name="parentuid">long, Tämän objektin vanhemman UID tieto ObjectIndexerissä </param>
        /// <param name="uniquerefnume">Long, (OwnUID) Slotin uniikki objektin numero - järjestysnumero objektille, jonka Object Indexer on antanut, eli käytännössä kohteen UID</param>
        /// <param name="smbotti"></param>
        /// <param name="phm">ProgramHMI, käyttöliittymäluokan referenssi </param>
        /// <param name="savemarkedctrl">SaveMarkerController luokan referenssi, jonne voidaan syöttää tieto muuttuneesta propertystä ja valvoa sitä kautta, tulisiko kohde tallentaa levylle</param>
        /// <param name="slotareap">int, Slottilistan yksilöllinen indeksi slotlistarrayssa, eli käytännössä Areapointer</param>
        /// <param name="slotvalue">decimal, Pitää sisällään sen kohdan kurssiarvosta, johon slotti luotiin</param>
        /// <param name="uniqueidstring">string, DeviceString, BotIndex ja Launchnumber yhdistettynä tuottaa slotlistpereid muuttujan ja siihen vielä lisätään SlotlistUniqueIndex viittaamaan mikä slotlistarray:n lista on kyseessä sekä SlotUniqueCrationNumber merkkaamaan slotin yksilöllisyyttä, josta saadaan SlotUniqueId aikaiseksi</param>
        /// <param name="slotuniqidnum">long, Tämän ei ole tarkoitus muuttua sitten luontihetken. Slotille yksilöllinen luontijärjestysnumero, mutta ei kuitenkaan UID vaan slotin luonnin järjestysnumero joka kasvaa yhdellä joka kerta kun slottilistaan on luotu uusi kohde. Tällä numerolla kohde löytyy varsinaisesti slottilistasta, mutta tämä on eri numero, kuin millä kohde löytyy ObjectIndexeristä. </param>
        /// <param name="objind"> ObjectIndexer, luokan referenssi, joka ottaa ylös säilytettäväkseen objektien yksilölliset id:t ja jakaa yksilöllisiä id numeroita ulospäin kysyttäessä</param>
        /// <param name="isOldMode"> int, tämä muuttuja kertoo onko kohteen toiminta vanhassa moodissa, jolloin sillä voi olla vain yksi AltRoute tieto (jos tämä muuttuja on 0) ja useita yhtäaikaisia AltRoute tietoja, jos tämä muuttuja on 1 </param>
        /// <param name="maxatt">int, Kuinka monta attemptia/reattemptia voidaan tehdä yhden slotin alueella maksimissaan (tämä luku rajoittaa arrayn kokoa reattempteille, joten ei tätä lukua enempää jokatapauksessa)</param>
        /// <returns> {void} </returns>
        public OneSlot(string kutsuja, long parentuid, long uniquerefnume, SmartBot smbotti, ProgramHMI phm, SaveMarkedController savemarkedctrl, int slotareap, decimal slotvalue, string uniqueidstring, long slotuniqidnum, ObjectIndexer objind, int isOldMode, int maxatt=1)
        {
            string functionname="->(OS)Constructor";
            this.phmi=phm;
            this.smartbotikka=smbotti;
            this._savemarkedctrl=savemarkedctrl; // SaveMarkerController luokan referenssi, jonne voidaan syöttää tieto muuttuneesta propertystä ja valvoa sitä kautta, tulisiko kohde tallentaa levylle
            this.objindexer=objind;

            this.ObjUniqueRefNum=uniquerefnume; // Long, Slotin uniikki objektin numero - järjestysnumero objektille, jonka Object Indexer on antanut
            this.ParentUID=parentuid; // Long, 
            int j=0;

            this.sellerobject = new TriggerRefObject(-1,phm,slotvalue,slotareap,uniqueidstring,slotuniqidnum); // Luodaan seller side object asseteille
            this.SellerObjUniqueNum=-1; // Long, Jos -1, niin UniqIdNum:ia ei ole asetettu vielä missään vaiheessa.

            this.holdingobject = new NormalRefObject(phm);
            this.blockinstructobj = new AttemptAndInstructionObject(kutsuja+functionname,this.phmi,this.ObjUniqueRefNum,this.objindexer,slotareap,isOldMode); // Luodaan objekti, joka pitää sisällään Altroute sekä Block Instructions tiedot

            if (maxatt<1) maxatt=1; 
            this.maxattempts=maxatt; // Int

            this.slotcreationnumber=slotuniqidnum; // Long
            this.slotuniqid=uniqueidstring; // String
            this.SlotAreap=slotareap; // Int
            this.SlotValue=slotvalue; // Decimal, Luo slotin, jonka value on slvalue
            this.SlotStatus=-1; // Int
            this.IsOrderAttachedType=0; // Int
            this.OrderIdNum=-1; // Long
            this.OrderIndex=-1; // Long
            this.OrderTrueValue=-1; // Decimal
            this.UnderOperation=-1; // Int
            this.SoldBought=-1; // Int
            this.IsCancelling=false; // Bool
            this.SlotSize=-1; // Decimal
            this.blockinstructobj.ResetSingleAltRoute(kutsuja+functionname); // Altroute=Int=-1 Clear käskyn jälkeen
            this.FirstTriggerValue=-1; // Decimal
            this.UpperTriggerSellPoint=-1; // Decimal
            this.UpperTriggerSafetyLimit=-1; // Decimal
            this.LowerTriggerSellPoint=-1; // Decimal
            this.PremioTriggerSellPoint=-1; // Decimal
            this.TriggerReserved=-1; // Int
            this.IsConditionalOrder=false; // Bool
            this.markercounter=-1; // Long
            this.markercounteratcreation=-1; // Long
            this.extraattemptsaltroutes = new int[maxattempts];
            this.firstriggervalues = new decimal[maxattempts];
            this.uppertriggersellingpoints = new decimal[maxattempts];
            this.lowertriggersellingpoints = new decimal[maxattempts];
            this.triggerorderindexes = new long[maxattempts];
            this.triggerreserved = new int[maxattempts];
            for (j=0; j<maxattempts; j++) {
                this.extraattemptsaltroutes[j]=-1;
                this.firstriggervalues[j]=-1;
                this.uppertriggersellingpoints[j]=-1;
                this.lowertriggersellingpoints[j]=-1;
                this.triggerorderindexes[j]=-1;
                this.triggerreserved[j]=-1;
            }

            // This list should be updated if more parameters are added or the order changes.
            this.allParamNames = new List<string>() {
                "ObjUniqueRefNum", "SlotValue", "OrderTrueValue", "IsOrderAttachedType", "IsOrderAttached", "SlotCreationNumber", "SlotUniqueId", "SlotAreap", "OrderIdNum", "MarkerCounter",
                "MarkerCounterAtCreation", "OrderIndex", "SlotStatus", "UnderOperation", "SoldBought", "IsCancelling", "SlotSize", "AltRoute", "FirstTriggerValue", "UpperTriggerSellPoint", "UpperTriggerSafetyLimit",
                "LowerTriggerSellPoint", "PremioTriggerSellPoint", "TriggerReserved", "IsConditionalOrder", "SellerObjUniqueNum"
            };

            this.helperlist = new List<string>();          
        }

        /// <summary> Palauttaa parametrien maksimimäärän, joiden nimet on tallennettu allParamNames listaan </summary>
        /// <returns> {int} parametrien maksimimäärä </returns>
        public int AllParamNamesMaxCount()
        {
            return this.allParamNames.Count;
        }

        /// <summary>
        /// Suorittaa OneSlot-objektin ohjeet = kokoelma sääntöjä jonka mukaan objekti toimii | Executes the instructions for the OneSlot object
        /// </summary>
        /// <param name='caller'>string, kutsujan polku | string, the caller's path</param>
        /// <param name="acUI">ActionCentreUI, sen ActionCentreUI instanssin referenssi, jota kautta päästään blokkikonstruktioon käsiksi, joka meidän tulee lukea läpi
        /// <returns> {void} </returns>
        public void RunInstruction(string caller, ActionCentreUI acUI)
        {
            string functionname = "->(OS)RunInstruction";

            acUI.RunStepEngineInstructions(caller+functionname,this);
        }        

        /// <summary> Method to set the OneSlotParamValues </summary>
        /// <param name="kutsuja">A string representing the caller's path.</param>
        /// <param name="paramName">A string representing the name of the parameter.</param>
        /// <param name="paramValue">A string representing the value of the parameter.</param>
        /// <returns> An integer representing the success or failure of the operation.</returns>
        public int SetOneSlotParamValues(string kutsuja, string paramName, string paramValue, int arrayIndex=-1)
        {
            int retVal=-1;
            string functionName = "->(OS)SetOneSlotParamValues";
            long lVal=-1;
            int iVal=-1;
            decimal dVal=-1;
            bool bVal=false;

            // For array properties
            if (arrayIndex >= 0 && arrayIndex < this.extraattemptsaltroutes.Length)
            {
                switch (paramName)
                {
                    case "extraattemptsaltroutes":
                        if (int.TryParse(paramValue, out extraattemptsaltroutes[arrayIndex]))
                        {
                            retVal = 1;
                        }
                        else
                        {
                            retVal = -202;
                            ErrorFunction(kutsuja + functionName, paramName, paramValue, retVal);                            
                        }
                        break;
                    case "firstriggervalues":
                        if (decimal.TryParse(paramValue, out firstriggervalues[arrayIndex]))
                        {
                            retVal = 3;
                        }
                        else
                        {
                            retVal = -203;
                            ErrorFunction(kutsuja + functionName, paramName, paramValue,retVal);
                        }
                        break;
                    case "uppertriggersellingpoints":
                        if (decimal.TryParse(paramValue, out firstriggervalues[arrayIndex]))
                        {
                            retVal = 3;
                        }
                        else
                        {
                            retVal = -204;
                            ErrorFunction(kutsuja + functionName, paramName, paramValue, retVal);
                        }
                        break;                    
                    case "lowertriggersellingpoints":
                        if (decimal.TryParse(paramValue, out firstriggervalues[arrayIndex]))
                        {
                            retVal = 3;
                        }
                        else
                        {
                            retVal = -205;
                            ErrorFunction(kutsuja + functionName, paramName, paramValue, retVal);
                        }
                        break;                    
                    case "triggerorderindexes":
                        if (long.TryParse(paramValue, out triggerorderindexes[arrayIndex]))
                        {
                            retVal = 2;
                        }
                        else
                        {
                            retVal = -206;
                            ErrorFunction(kutsuja + functionName, paramName, paramValue, retVal);
                        }
                        break;                    
                    case "triggerreserved":                        
                        if (int.TryParse(paramValue, out triggerreserved[arrayIndex]))
                        {
                            retVal = 1;
                        }
                        else
                        {
                            retVal = -207;
                            ErrorFunction(kutsuja + functionName, paramName, paramValue, retVal);
                        }
                        break;
                    default:
                        retVal = -108;
                        ErrorFunction(kutsuja + functionName, paramName, paramValue, retVal);
                        break;
                }
            } else {
                switch (paramName)
                {
                    case "ObjUniqueRefNum":
                        if (long.TryParse(paramValue, out lVal))
                        {
                            this.ObjUniqueRefNum = lVal;
                            retVal=2;
                        }
                        else
                        {
                            retVal=-2;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "SellerObjUniqueNum":
                        if (long.TryParse(paramValue, out lVal))
                        {
                            this.SellerObjUniqueNum = lVal;
                            retVal=2;
                        }
                        else
                        {
                            retVal=-3;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "maxattempts":
                        if (int.TryParse(paramValue, out iVal))
                        {
                            this.maxattempts = iVal;
                            retVal=1;
                        }
                        else
                        {
                            retVal=-4;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "SlotCreationNumber":
                        if (long.TryParse(paramValue, out lVal))
                        {
                            this.slotcreationnumber = lVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-5;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "SlotUniqueId":
                        this.slotuniqid = paramValue;
                        retVal=0;
                        break;
                    case "SlotAreap":
                        if (int.TryParse(paramValue, out iVal))
                        {
                            this.SlotAreap = iVal;
                            retVal=1;
                        }
                        else
                        {
                            retVal=-6;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "SlotValue":
                        if (decimal.TryParse(paramValue, out dVal))
                        {
                            this.SlotValue = dVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-7;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "SlotStatus":
                        if (int.TryParse(paramValue, out iVal))
                        {
                            this.SlotStatus = iVal;
                            retVal=1;
                        }
                        else
                        {
                            retVal=-8;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "IsOrderAttachedType":
                        if (int.TryParse(paramValue, out iVal))
                        {
                            this.IsOrderAttachedType = iVal;
                            retVal=1;
                        }
                        else
                        {
                            retVal=-9;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "OrderIdNum":
                        if (long.TryParse(paramValue, out lVal))
                        {
                            this.OrderIdNum = lVal;
                            retVal=2;
                        }
                        else
                        {
                            retVal=-10;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "OrderIndex":
                        if (long.TryParse(paramValue, out lVal))
                        {
                            this.OrderIndex = lVal;
                            retVal=2;
                        }
                        else
                        {
                            retVal=-11;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "OrderTrueValue":
                        if (decimal.TryParse(paramValue, out dVal))
                        {
                            this.OrderTrueValue = dVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-12;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "SlotSize":
                        if (decimal.TryParse(paramValue, out dVal))
                        {
                            this.SlotSize = dVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-13;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "FirstTriggerValue":
                        if (decimal.TryParse(paramValue, out dVal))
                        {
                            this.FirstTriggerValue = dVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-14;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "UpperTriggerSellPoint":
                        if (decimal.TryParse(paramValue, out dVal))
                        {
                            this.UpperTriggerSellPoint = dVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-15;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "UpperTriggerSafetyLimit":
                        if (decimal.TryParse(paramValue, out dVal))
                        {
                            this.UpperTriggerSafetyLimit = dVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-16;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "LowerTriggerSellPoint":
                        if (decimal.TryParse(paramValue, out dVal))
                        {
                            this.LowerTriggerSellPoint = dVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-17;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "PremioTriggerSellPoint":
                        if (decimal.TryParse(paramValue, out dVal))
                        {
                            this.PremioTriggerSellPoint = dVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-18;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "UnderOperation":
                        if (int.TryParse(paramValue, out iVal))
                        {
                            this.UnderOperation = iVal;
                            retVal=1;
                        }
                        else
                        {
                            retVal=-19;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "SoldBought":
                        if (int.TryParse(paramValue, out iVal))
                        {
                            this.SoldBought = iVal;
                            retVal=1;
                        }
                        else
                        {
                            retVal=-20;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "AltRoute":
                        if (int.TryParse(paramValue, out iVal))
                        {
                            this.AltRoute = iVal;
                            retVal=1;
                        }
                        else
                        {
                            retVal=-21;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "TriggerReserved":
                        if (int.TryParse(paramValue, out iVal))
                        {
                            this.TriggerReserved = iVal;
                            retVal=1;
                        }
                        else
                        {
                            retVal=-22;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "IsCancelling":
                        if (bool.TryParse(paramValue, out bVal))
                        {
                            this.IsCancelling = bVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-23;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "IsConditionalOrder":
                        if (bool.TryParse(paramValue, out bVal))
                        {
                            this.IsConditionalOrder = bVal;
                            retVal=3;
                        }
                        else
                        {
                            retVal=-24;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break; 
                    case "MarkerCounter":
                        if (long.TryParse(paramValue, out lVal))
                        {
                            this.MarkerCounter = lVal;
                            retVal=2;
                        }
                        else
                        {
                            retVal=-25;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                    
                    case "MarkerCounterAtCreation":
                        if (long.TryParse(paramValue, out lVal))
                        {
                            this.markercounteratcreation = lVal;
                            retVal=2;
                        }
                        else
                        {
                            retVal=-26;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;
                    case "IsOrderAttached":
                        if (bool.TryParse(paramValue, out bVal))
                        {
                            this.IsOrderAttached = bVal;
                            retVal=5;
                        }
                        else
                        {
                            retVal=-27;
                            ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        }
                        break;                        
                    default:
                        ErrorFunction(kutsuja + functionName,paramName, paramValue, retVal);
                        retVal=-999; // default error return
                        break;
                }
            }
            return retVal;
        }

        /// <summary> Tämä funktio tekee käytännössä saman kuin sendError, mutta se eroteltiin näin, jotta tekoälyn oli helpompi luoda haluamanlaistaan listaa </summary>
        /// <param name="kutsuja">A string representing the caller's path.</param>
        /// <returns> {void} </returns>
        public void ErrorFunction(string kutsuja, string paramName, string paramValue, int errval)
        {
            string funimi="->(OS)ErrorFunction";
            this.phmi.sendError(kutsuja+funimi,"Error setting value for parameter: "+paramName+" with value: "+paramValue+" and errcode:"+errval,-724,4,4);
        }

        /// <summary>
        /// Metodi, joka muuntaa annetun indeksin parametrin nimeksi.
        /// </summary>
        /// <param name="kutsuja">A string representing the caller's path.</param>
        /// <param name="index">Indeksi, jonka perusteella parametrin nimi valitaan.</param>
        /// <returns> {string} Parametrin nimi merkkijonona. Jos virhe, niin palauttaa tyhjän merkkijonon! </returns>
        public string GetParamNameByIndex(string kutsuja, int index)
        {
            string functionname="->(OS)GetParamNameByIndex";
            // Check that the index is within the bounds of the list.
            if (index < 0 || index >= allParamNames.Count)
            {
                this.phmi.sendError(kutsuja+functionname,"Index out of bounds - no parameter found! Index:"+index,-720,4,4);
                return ""; // Or throw an error if you prefer.
            }

            return allParamNames[index];
        }

        /// <summary>
        /// This method generates a string containing the specified order parameters, separated by tabs or commas or formatted as JSON.
        /// Available parameters: "ObjUniqueRefNum", "SlotValue", "OrderTrueValue", "IsOrderAttachedType", "IsOrderAttached", "SlotCreationNumber", "SlotUniqueId", "SlotAreap", "OrderIdNum", "MarkerCounter",
        /// "MarkerCounterAtCreation", "OrderIndex", "SlotStatus", "UnderOperation", "SoldBought", "IsCancelling", "SlotSize", "AltRoute", "FirstTriggerValue", "UpperTriggerSellPoint", "UpperTriggerSafetyLimit",
        /// "LowerTriggerSellPoint", "PremioTriggerSellPoint", "TriggerReserved", "IsConditionalOrder", "SellerObjUniqueNum"
        /// </summary>
        /// <param name="kutsuja">A string representing the caller's path.</param>
        /// <param name="paramindex">int, Palautettavan parametrin indeksinumero</param>
        /// <param name="printcsvlist">If 0, prints parameters with their names, separated by tabs. If 1, prints just values separated by commas. If 2, prints parameters and their values as JSON object.</param>
        /// <returns>A single string containing the specified order parameters and their values in requested format. Palauttaa tyhjän stringin, jos virhe! </returns>
        public string MyOwnPrintSlots(string kutsuja, int paramIndex, int printcsvlist=0)
        {
            string functionname="->(OS)MyOwnPrintSlots#1";
            // Get the parameter name for the provided index.
            string paramName = GetParamNameByIndex(kutsuja+functionname, paramIndex);
            if (paramName == "")
            {
                this.phmi.sendError(kutsuja+functionname,"Index out of bounds - no parameter found! Index:"+paramIndex+" Printlist:"+printcsvlist,-721,4,4);
                return ""; // Or throw an error if you prefer.
            }

            this.helperlist.Clear();
            this.helperlist.Add(paramName);

            // Call the original method with a list containing only the one parameter name.
            return MyOwnPrintSlots(kutsuja+functionname, this.helperlist, printcsvlist);
        }

        /// <summary>
        /// This method generates a string containing the specified order parameters, separated by tabs or commas or formatted as JSON.
        /// Available parameters for slots: "ObjUniqueRefNum", "SlotValue", "OrderTrueValue", "IsOrderAttachedType", "IsOrderAttached", "SlotCreationNumber", "SlotUniqueId", "SlotAreap", "OrderIdNum", "MarkerCounter",
        /// "MarkerCounterAtCreation", "OrderIndex", "SlotStatus", "UnderOperation", "SoldBought", "IsCancelling", "SlotSize", "AltRoute", "FirstTriggerValue", "UpperTriggerSellPoint", "UpperTriggerSafetyLimit",
        /// "LowerTriggerSellPoint", "PremioTriggerSellPoint", "TriggerReserved", "IsConditionalOrder", "SellerObjUniqueNum"
        /// </summary>
        /// <param name="kutsuja">A string representing the caller's path.</param>
        /// <param name="paramnames">A list of strings representing the order parameter names to include in the output string.</param>
        /// <param name="printcsvlist">If 0, prints parameters with their names, separated by tabs. If 1, prints just values separated by commas. If 2, prints parameters and their values as JSON object.</param>
        /// <returns>A single string containing the specified order parameters and their values in requested format. Palauttaa tyhjän stringin, jos virhe! </returns>
        public string MyOwnPrintSlots(string kutsuja, List<string> paramnames, int printcsvlist=0)
        {
            string functionName = "->(SL)MyOwnPrintSlots#2";
            string callerPath = kutsuja + functionName;
            string paramval="";

            // Check if paramnames list is empty
            if (paramnames.Count == 0)
            {
                this.phmi.sendError(kutsuja+functionName,"Paramnames list empty!",-687,4,4);
            }

            if (printcsvlist == (int)FillParentLevelObject.myOwnTypePrintingEnum.JSON_OBJECT_WITH_PARAM_NAMES_AND_VALUES_2)
            {
                // If JSON format is requested, use a Dictionary to store properties for serialization
                var properties = new Dictionary<string, string>();

                foreach (string paramName in paramnames)
                {
                    paramval=this.HandleOneSlotParamValues(kutsuja+functionName,paramName);
                    properties.Add(paramName,paramval);
                }

                // Serialize the properties to a JSON string
                string json = JsonSerializer.Serialize(properties);

                return json;
            }
            else
            {
                StringBuilder output = new StringBuilder();

                // Loop through paramnames list and add the parameter name and value to the output
                foreach (string paramName in paramnames)
                {
                    if (printcsvlist == (int)FillParentLevelObject.myOwnTypePrintingEnum.LIST_OF_PARAM_NAMES_AND_PARAMS_WITH_TABBING_0 || printcsvlist == (int)FillParentLevelObject.myOwnTypePrintingEnum.CSV_LIST_WITH_ONLY_PARAM_NAMES_3)
                    {
                        output.Append(paramName);

                        if (printcsvlist == (int)FillParentLevelObject.myOwnTypePrintingEnum.LIST_OF_PARAM_NAMES_AND_PARAMS_WITH_TABBING_0)
                        {
                            output.Append(":");
                        }
                    }

                    if (printcsvlist != (int)FillParentLevelObject.myOwnTypePrintingEnum.CSV_LIST_WITH_ONLY_PARAM_NAMES_3)
                    {
                        // Use switch statement to check the parameter name and add its value to the output
                        output.Append(this.HandleOneSlotParamValues(kutsuja+functionName,paramName));
                    }

                    // Add a tab after each parameter, except the last one
                    if (paramnames.IndexOf(paramName) < paramnames.Count - 1)
                    {
                        if (printcsvlist == (int)FillParentLevelObject.myOwnTypePrintingEnum.LIST_OF_PARAM_NAMES_AND_PARAMS_WITH_TABBING_0)
                        {
                            output.Append("\t");
                        }
                        else if (printcsvlist == (int)FillParentLevelObject.myOwnTypePrintingEnum.CSV_LIST_WITHOUT_PARAM_NAMES_1)
                        {
                            output.Append(",");
                        }
                    }
                }

                return output.ToString();
            }
        }

        /// <summary>
        /// Method to handle the OneSlotParamValues
        /// </summary>
        /// <param name="kutsuja">A string representing the caller's path.</param>
        /// <param name="paramName">string, parameter name which value we want to return as string</param>
        /// <param name="arrayIndex">int, An integer representing the index of the array from which to retrieve the information. </param>
        /// <returns> {string} A string representing the requested property value.</returns>
        public string HandleOneSlotParamValues(string kutsuja, string paramName, int arrayIndex=-1)
        {
            string functionName = "->(SL)HandleOneSlotParamValues";
            string retVal = "";
            
            // Check that the arrayIndex is within bounds
            if (arrayIndex >= 0 && arrayIndex < uppertriggersellingpoints.Length)
            {
                // Retrieve information from arrays
                int altRoute = extraattemptsaltroutes[arrayIndex];
                decimal upperTriggerSellingPoint = uppertriggersellingpoints[arrayIndex];
                decimal firstTriggerValue = firstriggervalues[arrayIndex];
                decimal lowerTriggerSellingPoint = lowertriggersellingpoints[arrayIndex];
                long triggerOrderIndex = triggerorderindexes[arrayIndex];
                int triggerReserved = triggerreserved[arrayIndex];
                
                // Do something with these values
                
                // Check the param name and return the relevant property
                switch (paramName)
                {
                    case "extraattemptsaltroutes":
                        retVal = altRoute.ToString();
                        break;
                    case "uppertriggersellingpoints":
                        retVal = upperTriggerSellingPoint.ToString();
                        break;
                    case "firstriggervalues":
                        retVal = firstTriggerValue.ToString();
                        break;
                    case "lowertriggersellingpoints":
                        retVal = lowerTriggerSellingPoint.ToString();
                        break;
                    case "triggerorderindexes":
                        retVal = triggerOrderIndex.ToString();
                        break;
                    case "triggerreserved":
                        retVal = triggerReserved.ToString();
                        break;
                    default:
                        this.phmi.sendError(kutsuja+functionName,"Wrong parameter name! ParamName:"+paramName,-689,4,4);
                        retVal="ERROR=-107";
                        break;
                }
            } else {
                // Access non-array properties
                switch (paramName)
                {
                    case "SlotSize":
                        retVal = SlotSize.ToString();
                        break;
                    case "SlotAreap":
                        retVal = SlotAreap.ToString();
                        break;
                    case "UnderOperation":
                        retVal = UnderOperation.ToString();
                        break;
                    case "ObjUniqueRefNum": 
                        retVal=this.ObjUniqueRefNum.ToString();
                        break;
                    case "SlotValue": 
                        retVal=this.SlotValue.ToString();
                        break;
                    case "OrderTrueValue": 
                        retVal=this.OrderTrueValue.ToString();
                        break;
                    case "IsOrderAttachedType": 
                        retVal=this.IsOrderAttachedType.ToString();
                        break;
                    case "IsOrderAttached": 
                        retVal=this.IsOrderAttached.ToString();
                        break;
                    case "SlotCreationNumber": 
                        retVal=this.SlotCreationNumber.ToString();
                        break;
                    case "SlotUniqueId": 
                        retVal=this.SlotUniqueId.ToString();
                        break;
                    case "OrderIdNum": 
                        retVal=this.OrderIdNum.ToString();
                        break;
                    case "MarkerCounter": 
                        retVal=this.MarkerCounter.ToString();
                        break;
                    case "MarkerCounterAtCreation": 
                        retVal=this.MarkerCounterAtCreation.ToString();
                        break;
                    case "OrderIndex": 
                        retVal=this.OrderIndex.ToString();
                        break;
                    case "SlotStatus": 
                        retVal=this.SlotStatus.ToString();
                        break;
                    case "SoldBought": 
                        retVal=this.SoldBought.ToString();
                        break;
                    case "IsCancelling": 
                        retVal=this.IsCancelling.ToString();
                        break;
                    case "AltRoute": 
                        retVal=this.AltRoute.ToString();
                        break;
                    case "FirstTriggerValue": 
                        retVal=this.FirstTriggerValue.ToString();
                        break;
                    case "UpperTriggerSellPoint": 
                        retVal=this.UpperTriggerSellPoint.ToString();
                        break;
                    case "UpperTriggerSafetyLimit": 
                        retVal=this.UpperTriggerSafetyLimit.ToString();
                        break;
                    case "LowerTriggerSellPoint": 
                        retVal=this.LowerTriggerSellPoint.ToString();
                        break;
                    case "PremioTriggerSellPoint": 
                        retVal=this.PremioTriggerSellPoint.ToString();
                        break;
                    case "TriggerReserved": 
                        retVal=this.TriggerReserved.ToString();
                        break;
                    case "IsConditionalOrder": 
                        retVal=this.IsConditionalOrder.ToString();
                        break;
                    case "SellerObjUniqueNum": 
                        retVal=this.SellerObjUniqueNum.ToString();
                        break;
                    default:
                        this.phmi.sendError(kutsuja+functionName,"Wrong parameter name! ParamName:"+paramName,-688,4,4);
                        retVal="ERROR=-106";
                        break;
                }
            }
            return retVal;
        }  

        public void ClearSlot(string kutsuja)
        {
            string functionname="->(OS)ClearSlot";
            int j=0;
            // this.SlotValue=-1; // SlotValue ei muutu, muuten tyhjätään tiedot
            // this.ObjUniqueRefNum=-1; Ei saa tyhjätä, koska slotvaluekaan ei muutu
            // this.SlotUniqueNumber=-1;
            // this.SlotUniqueId=-1;
            // this.SlotAreap=-1;

            //this.tryToCancelSlotAssets(kutsuja+funimi,assrettype,assetdatasilorefe,prhmi); // Yritetään peruuttaa meidän slotin assetit
            //this.SellerObjUniqueNum=-1;

            this.SlotStatus=-1;
            this.IsOrderAttachedType=0;
            this.OrderIdNum=-1;
            this.OrderIndex=-1;
            this.OrderTrueValue=-1;
            this.UnderOperation=-1;
            this.SoldBought=-1;
            this.IsCancelling=false;
            this.SlotSize=-1;
            this.blockinstructobj.ResetSingleAltRoute(kutsuja+functionname); // Altroute=Int=-1 Clear käskyn jälkeen
            this.markercounter=-1;
            this.markercounteratcreation=-1;
            this.FirstTriggerValue=-1;
            this.UpperTriggerSellPoint=-1;
            this.UpperTriggerSafetyLimit=-1;            
            this.LowerTriggerSellPoint=-1;
            this.PremioTriggerSellPoint=-1;
            this.TriggerReserved=-1;
            this.IsConditionalOrder=false;            
            for (j=0; j<maxattempts; j++) {
                this.extraattemptsaltroutes[j]=-1;
                this.firstriggervalues[j]=-1;
                this.uppertriggersellingpoints[j]=-1;
                this.lowertriggersellingpoints[j]=-1;
                this.triggerorderindexes[j]=-1;
                this.triggerreserved[j]=-1;                
            }            
        }

        /// <summary> Tämä funktio resetoi slottia ensimmäisen triggerlistaus stepin jälkeen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="assrettype"> int, minkätyyppisiä assetteja perutaan, -1=ohitetaan assettien peruutus, 0=sellerobjektin kautta holding objektit, 1=holdingobjektin kautta sellerobjektit </param>
        /// <param name="assetdatasilorefe"> AssetDataSilo, sen assetdatasilon referenssi, joka ylläpitää seller ja holding kohteita </param>
        /// <param name="prhmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="cancelthis"> bool, merkataanko kohde cancelloitavaksi vai ei </param>
        /// <param name="printstr"> string, printattavien tietojen esitiedot </param>
        /// <returns> {string} printattavat tiedot stringinä </returns>
        public string ResetSlotAfterFirstTriggerStep(string kutsuja, int assrettype, AssetDataSilo assetdatasilorefe, ProgramHMI prhmi, bool cancelthis=false, string printstr="", decimal currcourse=-1)
        {
            string funimi="->(OS)ResetSlotAfterFirstTriggerStep";
            string retVal=""; // Palautetaan vain, jos halutaan debug tietoa
            int j=0;
            int ans=-1;

            // SlotValue ei muutu, muuten tyhjätään tiedot
            if (printstr!="") {
                retVal=printstr+"Before reseting: "+this.ObjectInfoString(kutsuja+funimi,currcourse);
            } 

            if (assrettype>=0) { // Jos ei ohiteta assettien peruuttamista
                this.TryToCancelSlotAssets(kutsuja+funimi,assrettype,assetdatasilorefe,prhmi); // Yritetään peruuttaa meidän slotin assetit
                this.SellerObjUniqueNum=-1;
            }

            ans=assetdatasilorefe.checkIfNoSellers(kutsuja+funimi); // Tarkistetaan hävisikö viimeisinkin sellerobject listoilta, jolloin nollataan tilanne
            if (ans<0) {
                prhmi.sendError(kutsuja+funimi,"Couldn't check seller amount - Error:"+ans+" ObjUniqueRefNum:"+this.ObjUniqueRefNum+" SlotVal:"+this.SlotValue+" Areap:"+this.SlotAreap+" SlotCreationNumber(SlotUniqNum):"+this.SlotCreationNumber+" SlotUniqId:"+this.SlotUniqueId+" OrderId:"+OrderIdNum+" OrderIndex:"+this.OrderIndex,-291);
            }

            // this.SlotValue=-1; // SlotValue ei muutu, muuten tyhjätään tiedot
            // this.ObjUniqueRefNum=-1; Ei saa tyhjätä, koska slotvaluekaan ei muutu
            // this.SlotUniqueNumber=-1;
            // this.SlotUniqueId=-1;
            // this.SlotAreap=-1;

            this.SlotStatus=-1;
            this.IsOrderAttachedType=0;
            this.OrderIdNum=-1;
            this.OrderIndex=-1;
            this.OrderTrueValue=-1;
            this.UnderOperation=-1;
            this.SoldBought=-1;
            this.IsCancelling=cancelthis;
            //this.SlotSize=-1;
            //this.AltRoute=-1;
            this.markercounter=-1;
            this.markercounteratcreation=-1;
            //this.FirstTriggerValue=-1;
            //this.UpperTriggerSellPoint=-1;
            //this.UpperTriggerSafetyLimit=-1;
            //this.LowerTriggerSellPoint=-1;
            //this.PremioTriggerSellPoint=-1;
            //this.TriggerReserved=-1;
            //this.IsConditionalOrder=false;
            for (j=0; j<maxattempts; j++) {
                this.extraattemptsaltroutes[j]=-1;
                this.firstriggervalues[j]=-1;
                this.uppertriggersellingpoints[j]=-1;
                this.lowertriggersellingpoints[j]=-1;
                this.triggerorderindexes[j]=-1;
                this.triggerreserved[j]=-1;                
            }

            return retVal;      
        }

        /// <summary> Tämä funktio resetoi slotin triggerlistaus steppien jälkeen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="assrettype"> int, minkätyyppisiä assetteja perutaan, 0=sellerobjektin kautta holding objektit, 1=holdingobjektin kautta sellerobjektit </param>
        /// <param name="assetdatasilorefe"> AssetDataSilo, sen assetdatasilon referenssi, joka ylläpitää seller ja holding kohteita </param>
        /// <param name="prhmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="printstr"> string, printattavien tietojen esitiedot </param>
        /// <returns> {string}, printattavat tiedot stringinä </returns>
        public string ResetSlotAfterAllTriggerStep(string kutsuja, int assrettype, AssetDataSilo assetdatasilorefe, ProgramHMI prhmi, string printstr="", decimal currcourse=-1)
        {
            string funimi="->(OS)resetSlotAfterAllTriggerStep";
            string retVal="";
            int j=0;
            int ans=-1;

            // SlotValue ei muutu, muuten tyhjätään tiedot
            if (printstr!="") {
                retVal=printstr+"Before reseting: "+this.ObjectInfoString(kutsuja+funimi,currcourse);
            }

            this.TryToCancelSlotAssets(kutsuja+funimi,assrettype,assetdatasilorefe,prhmi); // Yritetään peruuttaa meidän slotin assetit
            this.SellerObjUniqueNum=-1;

            ans=assetdatasilorefe.checkIfNoSellers(kutsuja+funimi); // Tarkistetaan hävisikö viimeisinkin sellerobject listoilta, jolloin nollataan tilanne
            if (ans<0) {
                prhmi.sendError(kutsuja+funimi,"Couldn't check seller amount - Error:"+ans+" ObjUniqueRefNum:"+this.ObjUniqueRefNum+" SlotVal:"+this.SlotValue+" Areap:"+this.SlotAreap+" SlotCreationNum(UniqNum):"+this.SlotCreationNumber+" SlotUniqId:"+this.SlotUniqueId+" OrderId:"+OrderIdNum+" OrderIndex:"+this.OrderIndex,-292);
            }            

            // this.SlotValue=-1; // SlotValue ei muutu, muuten tyhjätään tiedot
            // this.ObjUniqueRefNum=-1; Ei saa tyhjätä, koska slotvaluekaan ei muutu
            // this.SlotUniqueNumber=-1;
            // this.SlotUniqueId=-1;
            // this.SlotAreap=-1;

            this.SlotStatus=-1;
            this.IsOrderAttachedType=0;
            this.OrderIdNum=-1;
            this.OrderIndex=-1;
            this.OrderTrueValue=-1;
            this.UnderOperation=-1;
            this.SoldBought=-1;
            this.IsCancelling=false;
            this.SlotSize=-1;
            this.blockinstructobj.ResetSingleAltRoute(kutsuja+funimi); // Altroute=Int=-1 Clear käskyn jälkeen
            this.markercounter=-1;
            this.markercounteratcreation=-1;
            this.FirstTriggerValue=-1;
            this.UpperTriggerSellPoint=-1;
            this.UpperTriggerSafetyLimit=-1;
            this.LowerTriggerSellPoint=-1;
            this.PremioTriggerSellPoint=-1;
            this.TriggerReserved=-1;

            this.FirstTriggerValue=-1;
            this.UpperTriggerSellPoint=-1;
            this.UpperTriggerSafetyLimit=-1;
            this.LowerTriggerSellPoint=-1;
            this.PremioTriggerSellPoint=-1;
            this.TriggerReserved=-1;

            this.IsConditionalOrder=false;
            for (j=0; j<maxattempts; j++) {
                this.extraattemptsaltroutes[j]=-1;
                this.firstriggervalues[j]=-1;
                this.uppertriggersellingpoints[j]=-1;
                this.lowertriggersellingpoints[j]=-1;
                this.triggerorderindexes[j]=-1;
                this.triggerreserved[j]=-1;                
            }

            return retVal;            
        }

        public string ObjectInfoString(string kutsuja, decimal currcourse)
        {
            // Tämä funktio printtaa objektin info tiedot
            string printstr="";
            string extrstr="";
            if (currcourse>=0) {
                extrstr="CourseNow: "+currcourse+" ";
            }
            printstr=extrstr+"ObjUniqRef:"+this.ObjUniqueRefNum+" Altr:"+this.AltRoute+" OrderID:"+this.OrderIdNum+" OrderInd:"+this.OrderIndex+" TrueVal:"+this.OrderTrueValue+" Sig:"+this.markercounter+" IsCanc.:"+this.IsCancelling+" IsAtt:"+this.IsOrderAttachedType+" Trig.Res:"+this.TriggerReserved+" FTV:"+this.FirstTriggerValue+" UTSP:"+this.UpperTriggerSellPoint+" LTSP:"+this.LowerTriggerSellPoint+" SLTSP:"+(this.LowerTriggerSellPoint-this.smartbotikka.SafeLowerDistance)+" PTSP:"+this.PremioTriggerSellPoint+" SPTSP:"+(this.PremioTriggerSellPoint-this.smartbotikka.SafePremioDistance)+" UTSL:"+this.UpperTriggerSafetyLimit;
            return printstr;
        }

        /// <summary> Tämä funktio yrittää palauttaa slotin assetit cancelloinnin yhteydessä </summary>
        /// <param name="kutsuja"> = string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="assrettype"> = int, minkätyyppisiä assetteja perutaan, 0=sellerobjektin kautta holding objektit, 1=holdingobjektin kautta palautetaan rahat sellerobjekteille </param>
        /// <param name="assetdatasilorefe"> = AssetDataSilo, sen assetdatasilon referenssi, joka ylläpitää seller ja holding kohteita </param>
        /// <param name="prhmi"> = ProgramHMI, käyttöliittymän referenssi </param>
        private void TryToCancelSlotAssets(string kutsuja,int assrettype, AssetDataSilo assetdatasilorefe, ProgramHMI prhmi)
        {
            string funimi="->(OS)TryToCancelSlotAssets";
            int succorn=-1;
            int patchrsit=-3;
            int amo=-1;
            int holans=-1;

            // Palautetaan rahat cancelloinnin tai myynnin tapauksessa kohteisiinsa (jos ei ole mikään tapaus listalta, niin ei peruta ollenkaan)
            if (assrettype==0) { // Sellerobjektin kautta perutaan holdingobjektit
                patchrsit=this.sellerobject.isPatchRefSet(kutsuja+funimi);
                if (patchrsit>0) { // Patchref on asetettu, joten kohteesta voi yrittää tuhota sisältöä
                    succorn=this.sellerobject.cancelOrRemoveOneSeller(kutsuja+funimi,assetdatasilorefe,1,0,true,false,true);
                    if (succorn<0) {
                        prhmi.sendError(kutsuja+funimi,"Unsuccesful cancelling of seller object! Error:"+succorn,-200);
                    }
                } else {
                    if (patchrsit<0) {
                        prhmi.sendError(kutsuja+funimi,"Problems with patchref! Error:"+patchrsit,-290);
                    }
                }
            } else if (assrettype==1) { // holding objektin kautta perutaan sellerobjektit
                amo=this.holdingobject.sharestorage.Count();
                if (amo>0) {
                    holans=this.holdingobject.cancelOrRemoveAllHoldings(kutsuja+funimi,assetdatasilorefe,1,true); // Palautetaan mahdolliset saadut lainat; 1=palauttaa rahat lainanantajille, true=poistaa holding objectin
                    if (holans<0 && holans!=-2) {                    
                        prhmi.sendError(kutsuja+funimi,"Couldn't remove all holding objects! Error:"+holans,-201); // Jotain epäonnistui holding objectien poistossa
                    }
                }
            }

        }

        /// <summary> Tämä funktio resetoi slotin, jos ostotapahtuma epäonnistui operaation automaattisen listalta perumisen vuoksi (esim. kurssi ennättänyt heilahtaa niin paljon, ettei suunniteltua ostoa voida suorittaa) </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="assrettype"> = int, minkätyyppisiä assetteja perutaan, -1=ohitetaan assettien poisto, 0=sellerobjektin kautta holding objektit, 1=holdingobjektin kautta palautetaan rahat sellerobjekteille </param>
        /// <param name="assetdatasilorefe"> = AssetDataSilo, sen assetdatasilon referenssi, joka ylläpitää seller ja holding kohteita </param>
        /// <param name="prhmi"> = ProgramHMI, käyttöliittymän referenssi </param>
        public int ResetAfterFailedBuying(string kutsuja, int assrettype, AssetDataSilo assetdatasilorefe, ProgramHMI prhmi)
        {
            string funimi="->(OS)resetAfterFailedBuying";
            int retVal=-4;
            int j=0;

            if (this.UnderOperation==1 && this.IsCancelling==false) {
                if (this.SlotStatus<1 && this.SoldBought<1) { // Tulisi täyttyä aina, kun tätä käskyä kysytään
                    if (this.orderidnum<1 && this.OrderIndex<0) { // Tulisi täyttyä aina, kun tätä käskyä kysytään

                        if (assrettype>=0) { // Jos ei ohiteta assettien peruuttamista
                            this.TryToCancelSlotAssets(kutsuja+funimi,assrettype,assetdatasilorefe,prhmi); // Yritetään palauttaa rahat seller kohteelle, joka on lainannut kyseiselle holding objectille
                        }

                        // this.SlotValue=-1; // SlotValue ei muutu, muuten tyhjätään tiedot
                        // this.ObjUniqueRefNum=-1; Ei saa tyhjätä, koska slotvaluekaan ei muutu
                        // this.SlotUniqueNumber=-1;
                        // this.SlotUniqueId=-1;
                        // this.SlotAreap=-1;

                        this.SlotStatus=-1;
                        this.IsOrderAttachedType=0;
                        this.OrderIdNum=-1;
                        this.OrderIndex=-1;
                        this.OrderTrueValue=-1;
                        this.UnderOperation=-1;
                        this.SoldBought=-1;
                        this.IsCancelling=false; 
                        this.SlotSize=-1;
                        this.blockinstructobj.ResetSingleAltRoute(kutsuja+funimi); // Altroute=Int=-1 Clear käskyn jälkeen 
                        this.markercounter=-1;
                        this.markercounteratcreation=-1; 
                        this.FirstTriggerValue=-1;
                        this.UpperTriggerSellPoint=-1;
                        this.UpperTriggerSafetyLimit=-1;
                        this.LowerTriggerSellPoint=-1;
                        this.PremioTriggerSellPoint=-1;
                        this.TriggerReserved=-1;
                        this.IsConditionalOrder=false;
                        for (j=0; j<maxattempts; j++) {
                            this.extraattemptsaltroutes[j]=-1;
                            this.firstriggervalues[j]=-1;
                            this.uppertriggersellingpoints[j]=-1;
                            this.lowertriggersellingpoints[j]=-1;
                            this.triggerorderindexes[j]=-1;
                            this.triggerreserved[j]=-1;                            
                        }                                                
                        retVal=1;
                    } else {
                        retVal=-3;
                    }
                } else {
                    retVal=-2;
                }
            } else {
                retVal=-1;
            }

            return retVal;
        }

        /// <summary> Tämä funktio resetoi slotin tiedot sen jälkeen, kun ostotapahtuma jäi jumiin liian nopeiden kurssiliikkeiden vuoksi, mutta se löydettiin Fills listalta </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        public int ResetBuyingAfterFindingFillsList(string kutsuja)
        {
            string functionname="->(OS)ResetBuyingAfterFindingFillList";

            int retVal=-1;
            if (this.OrderIdNum>=0 && this.OrderIndex<0) {
                this.SlotStatus=0; // Kohteen status on nolla
                this.IsOrderAttachedType=0;
                this.OrderIdNum=-1;
                this.OrderIndex=-1;
                this.OrderTrueValue=-1;
                this.UnderOperation=0; // Under operation on nolla
                this.SoldBought=1; // Kohde on ostettu
                this.IsCancelling=false; 
                // this.SlotSize=-1; - SlotSizeä ei saa nollata
                // this.ObjUniqRefNum=-1; 
                this.blockinstructobj.ResetSingleAltRoute(kutsuja+functionname); // Altroute=Int=-1 Clear käskyn jälkeen  
                this.markercounter=-1;
                this.markercounteratcreation=-1;              
                retVal=1;
            }

            return retVal;
        }
    }
