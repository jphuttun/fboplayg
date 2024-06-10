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

    /// <summary> Parametrisista toimintapoluista vastaava luokka, jolla voi hyödyntää blokkirakenteen toimintaobjekteja. Lisäksi säilyttää toimintablokkien sisältöjä listofallblocks sortedlist listassa. </summary>
    public class ActionCentre
    {
        /// <summary> Käyttöliittymästä vastaavan luokan referenssi </summary>
        private ProgramHMI proghmi;

        /// <summary> Enumeroidut valinnat valintalaatikolle, jossa vaihtoehtona on normaal IF lause (suurempi, pienempi, yhtäsuuri jne.) tai monimutkaisempi IF lause (sisällä, ulkona jne.) </summary>
        public enum blockTypeEnum {
            /// <summary> Normaali IF lause, jossa vaihtoehtoina suurempi, pienempi, yhtäsuuri, erisuuri jne. </summary>
            COMPARISON_BLOCK_NORMAL_IF_1=1,
            /// <summary> Monimutkaisempi IF lause, jossa vaihtoehtoina sisällä, ulkona jne. </summary>
            COMPARISON_BLOCK_BETWEEN_OUTSIDE_2=2,
            /// <summary> Blokki, joka antaa järjestelmälle alkuarvon järjestelmän sisäisistä tiedoista </summary>
            CODE_VALUE_BLOCK_100=100,
            /// <summary> Blokki, johon käyttäjä saa itse laittaa haluamanlaisensa alkuarvon </summary>
            OWN_VALUE_BLOCK_101=101,
            /// <summary> Matemaattisen operaation blokki, kuten esim. + - / * </summary>
            OPERATION_BLOCK_200=200,
            /// <summary> Matemaattisen operaation blokki, kuten esim. + - / * mutta kolmella muuttujan arvolla </summary>
            OPERATION_BLOCK_3_PROPERTY_201=201,            
            /// <summary> Tietoa blokin sisään syöttävä kahvablokki. Pakollinen kahvablokki, jolla blokeista voidaan tehdä suurempia kokonaisuuksia ja käyttää näitä suurempia kokonaisuuksia vähän samaan malliin kuin esim. ohjelmakoodin funktioita voidaan kutsua. Tällöin kaikille kahvablokin atomeille tulee pystyä syöttämään jokin tieto </summary>
            HANDLE_BLOCK_IN_300=300,
            /// <summary> Tietoa blokista ulkopuolelle syöttävä kahvablokki. Pakollinen kahvablokki, jolla blokeista voidaan tehdä suurempia kokonaisuuksia ja käyttää näitä suurempia kokonaisuuksia vähän samaan malliin kuin esim. ohjelmakoodin funktioita voidaan kutsua. Tällöin kaikille kahvablokin atomeille tulee pystyä syöttämään jokin tieto </summary>
            HANDLE_BLOCK_OUT_301=301,
            /// <summary>
            /// Blokki, jolla voidaan suorittaa Market Buy operaatio kauppapaikassa
            /// </summary>
            MARKET_BUY_BLOCK_400=400,
            /// <summary>
            /// Blokki, jolla voidaan suorittaa Market Sell operaatio kauppapaikassa
            /// </summary>
            MARKET_SELL_BLOCK_401=401,
            /// <summary>
            /// Blokki, jolla voidaan suorittaa Limit Buy operaatio kauppapaikassa
            /// </summary>
            LIMIT_BUY_BLOCK_402=402,
            /// <summary>
            /// Blokki, jolla voidaan suorittaa Limit Sell operaatio kauppapaikassa
            /// </summary>
            LIMIT_SELL_BLOCK_403=403,
            /// <summary>
            /// Blokki ActionCenterUI:hin, jolla voidaan suorittaa Check If Filled operaation kauppapaikassa halutulla markkinalla tekevä objekti
            /// </summary>
            TEST_IF_FILLED_BLOCK_500=500,
            /// <summary>
            /// Blokki ActionCenterUI:hin, jolla voidaan suorittaa MOWWM (MoveOrdersWithWrongMarker) operaatio meidän omassa ohjelmassa halutulla markkinalla tekevä objekti
            /// </summary>
            TEST_IF_REMOVED_MOWWM_BLOCK_600=600,
            /// <summary>
            /// Blokki ActionCenterUI:hin, jolla voidaan suorittaa jokin blokkien esiohjelmoitu resetointi funktio
            /// </summary>
            RESET_ALL_BLOCK_VALUES_700=700,
            /// <summary>
            /// Blokki ActionCenterUI:hin, jolla voidaan keskeyttää eteneminen blokissa siltä erää
            /// </summary>
            END_FOR_NOW_BLOCK_800=800
        }        

        /// <summary>
        /// ObjectIndexer, Object instance that give unique UID's to different objects.
        /// </summary>
        private ObjectIndexer objectindexer;

        /// <summary>
        /// Tehdasmetodin luokka, jolla luodaan kohteet. Tämä tehdasmetodi luokka ylläpitää myös listaa kaikista blokki muotoisista objekteista niiden UID:in perusteella
        /// </summary>
        private OperationalBlocksFactory operblocksfact;

        /// <summary>
        /// Tällä propertyllä saadaan palautettu tehdasmetodin luokka, jolla luodaan kohteet. Tämä tehdasmetodi luokka ylläpitää myös listaa kaikista blokki muotoisista objekteista niiden UID:in perusteella
        /// </summary>
        public OperationalBlocksFactory ReturnOperationalBlockFactoryRef {
            get { return this.operblocksfact; }
        }

        /// <summary> Constructor parametrisistä toimintapoluista vastaavalle luokalle </summary>
        /// <param name="kutsuja"> kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="prhmi"> ProgramHMI, käyttöliittymä luokan referenssi </param>
        /// <param name="objind">ObjectIndexer, Object instance that give unique UID's to different objects. </param>
        /// <returns> {void} </returns>
        public ActionCentre(string kutsuja, ProgramHMI prhmi, ObjectIndexer objind)
        {
            string functionname="->(AC)ActionCentre";
            this.proghmi=prhmi;
            this.objectindexer=objind;
            this.operblocksfact=new OperationalBlocksFactory(kutsuja+functionname,this.proghmi,this.objectindexer);
        }

        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <returns></returns> 
        public bool CheckIncomingHandles(string kutsuja, long UIDtoseek)
        {
            string functionname="->(AC)CheckIncomingHandles";
            if (this.operblocksfact.ReturnListOfAllBlocks.IndexOfKey(UIDtoseek)>-1) {
                retVal=this.operblocksfact.CheckIncomingHandles(kutsuja+functionname,UIDtoseek);
            } else {
                this.proghmi.sendError(kutsuja+functionname,"UID not found in listofallblocks! UIDtoseek:"+UIDtoseek,-1101,4,4);
            }
        }

        /// <summary>
        /// Etsii uid tiedolla blokkia ja palauttaa IOperationalBlocks &lt; T &gt; tyyppisen objektin BlockHandles luokan referenssin, joka ylläpitää kahvojen tietoja.
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <returns>{BlockHandles} Palauttaa IOperationalBlocks &lt; T &gt; tyyppisen objektin BlockHandles luokan referenssin, joka ylläpitää kahvojen tietoja. </returns>
        public BlockHandles GetBlockHandlesByUID(string kutsuja, long UIDtoseek)
        {
            string functionname = "->(AC)GetBlockHandlesByUID";
            return this.operblocksfact.GetBlockHandlesByUID(kutsuja+functionname,UIDtoseek);
        }        

        /// <summary>
        /// Create a block based on its type and UID. 
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="blockType">int, the type of the block to run.</param>
        /// <param name="motherconnrect"> MotherConnectionRectangle, kohteen äitiblokki, jonka kautta saadaan luettua prototyyppi blokille arvot sen toimintoja (kuten ComparsionBlock jne.) varten </param>
        /// <returns>{long} Palauttaa luodun BlockContainer tyyppisen kohteen UID:n. Palauttaa miinusmerkkisen arvon, jos Blokin luonti meni vikaan. | returns created object UID which type is BlockContainer</returns>
        public long CreateBlock(string caller, int blockType, MotherConnectionRectangle motherconnrect)
        {
            string functionname = "->(AC)RunBlock";

            long objectuid=-1;
            object newBlock = null;
            BlockContainer blockContainer= null;
            int objindtype=-1;
            OperationalBlocksFactoryParams obfparams;
            int routeid;
            string blockn;

            if (motherconnrect!=null) {
                long parentuid=-1;
                parentuid=motherconnrect.OwnUID;
                long granparentuid=motherconnrect.ParentUID;
                int parentobjtype=this.objectindexer.ReturnObjectType(caller+functionname,parentuid); 

                if (parentobjtype>=0) {

                    // Switch case based on blockType
                    switch (blockType)
                    {
                        // Normaali IF lause, jolla vertaillaan lukua/merkkijonoa johonkin
                        case (int)blockTypeEnum.COMPARISON_BLOCK_NORMAL_IF_1:
                            objindtype=(int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_COMPARISON_OBJECT_340;
                            obfparams = new OperationalBlocksFactoryParams{ 
                                ComparisonBlockType = (int)ComparisonBlock.ComparisonBlockTypeEnum.NORMAL_COMPARISON_1, 
                                SelectedHandle = (int)ComparisonBlock.SelectedHandleEnum.HANDLE_NOT_SET_0 
                            };
                            routeid=motherconnrect.StoredUIcomps.StoredParamValues.RouteId;
                            blockn=motherconnrect.StoredUIcomps.StoredParamValues.BlockName;
                            blockContainer=this.operblocksfact.CreateOperationalBlock<ComparisonBlock>(caller+functionname,this.proghmi,this.objectindexer,objindtype,parentuid,granparentuid, parentobjtype,routeid,blockn,motherconnrect,obfparams);
                            objectuid=blockContainer.Objectuid;
                            break;
                        // Laajennettu IF lause, jolla lukua verrataan, onko se jonkin lukualueen sisällä tai ulkopuolella
                        case (int)blockTypeEnum.COMPARISON_BLOCK_BETWEEN_OUTSIDE_2:
                            objindtype=(int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_COMPARISON_OBJECT_340;
                            obfparams = new OperationalBlocksFactoryParams{ 
                                ComparisonBlockType = (int)ComparisonBlock.ComparisonBlockTypeEnum.BETWEEN_OUTSIDE_COMPARISON_2, 
                                SelectedHandle = (int)ComparisonBlock.SelectedHandleEnum.HANDLE_NOT_SET_0 
                            };
                            routeid=motherconnrect.StoredUIcomps.StoredParamValues.RouteId;
                            blockn=motherconnrect.StoredUIcomps.StoredParamValues.BlockName;
                            blockContainer=this.operblocksfact.CreateOperationalBlock<ComparisonBlock>(caller+functionname,this.proghmi,this.objectindexer,objindtype,parentuid,granparentuid, parentobjtype,routeid,blockn,motherconnrect,obfparams);
                            objectuid=blockContainer.Objectuid;
                            break;
                        // Matemaattisen operaation blokki, josta tällä hetkellä löytyy +,-,/ ja * operaatiot
                        case (int)blockTypeEnum.OPERATION_BLOCK_200:
                            objindtype=(int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_OPERATIONBLOCK_OBJECT_341;
                            obfparams = new OperationalBlocksFactoryParams {
                                OperationalBlockType = (int)OperationBlock.OperationBlockTypeEnum.NORMAL_OPERATION_2,
                                SelectedHandle = (int)ComparisonBlock.SelectedHandleEnum.HANDLE_NOT_SET_0
                            };
                            routeid=motherconnrect.StoredUIcomps.StoredParamValues.RouteId;
                            blockn=motherconnrect.StoredUIcomps.StoredParamValues.BlockName;
                            blockContainer=this.operblocksfact.CreateOperationalBlock<OperationBlock>(caller+functionname,this.proghmi,this.objectindexer,objindtype,parentuid,granparentuid, parentobjtype,routeid,blockn,motherconnrect,obfparams);
                            objectuid=blockContainer.Objectuid;
                            break;
                            // ... Additional cases for other block types ...

                        default:
                            this.proghmi.sendError(caller + functionname, "Unknown block type: " + blockType, -1036, 4, 4);
                            objectuid=-10;
                            return objectuid;
                    }

                    if (objectuid<0) {
                        this.proghmi.sendError(caller + functionname, "ObjectUID was invalid: " +objectuid+" blocktype:"+ blockType, -1098, 4, 4);
                    }
                    return objectuid;
                } else {
                    this.proghmi.sendError(caller+functionname,"No parent object type was set! Response:"+parentobjtype+" ParentUID:"+parentuid,-1104,4,4);
                    objectuid=-14;
                    return objectuid;
                }
            } else {
                this.proghmi.sendError(caller + functionname, "Given mother connection rectangle was null: " + blockType, -1097, 4, 4);
                objectuid=-12;
                return objectuid;                
            }
        }        

    }

    /// <summary>
    /// Tämä luokka sitoo alaobjektin sekä objektin tyypin samaan olioon, jotta tältä luokalta voi kysyä suoraan, minkä tyyppinen objekti meillä on tallennettuna
    /// </summary>
    public class BlockContainer
    {

        private object blockobject;
        /// <summary> Tämä property palauttaa tai settaa itse objektin, mutta ei sen tyyppiä </summary>
        public object BlockObject {
            get {
                if (this.useobjindexer==false) {
                    return this.blockobject;
                } else {
                    if (this.objindexer.objectlist.IndexOfKey(this.Objectuid)>-1) {
                        return this.objindexer.objectlist[this.Objectuid].ObjectRef;
                    } else {
                        this.proghmi.sendError("BlockContainer -> BlockObject","There wasn't object in ObjectIndexer with such UID! Returning null! UID:"+this.Objectuid,-1106,4,4);
                        return null;
                    }
                }
            }
            set {
                if (this.useobjindexer==false) {
                    this.blockobject = value;
                } else {
                    if (this.objindexer.objectlist.IndexOfKey(this.Objectuid)>-1) {
                        this.objindexer.objectlist[this.Objectuid].ObjectRef=value;
                    } else {
                        this.proghmi.sendError("BlockContainer -> BlockObject","There wasn't object in ObjectIndexer with such UID! UID:"+this.Objectuid,-1107,4,4);
                    }
                }                    
            }
        }

        private int objecttype;
        /// <summary> Tämä property palauttaa tai settaa itse objektin tyypin, jonka perusteella objekti voidaan castata halaumanlaiseksi ja sitä voidaan käsitellä </summary>
        public int Objecttype {
            get {
                if (this.useobjindexer==false) {
                    return this.objecttype;
                } else {
                    if (this.objindexer.objectlist.IndexOfKey(this.Objectuid)>-1) {
                        return this.objindexer.ReturnObjectType("BlockContainer->Objecttype",this.Objectuid);
                    } else {
                        this.proghmi.sendError("BlockContainer -> Objecttype","There wasn't object in ObjectIndexer with such UID! Returning -1! UID:"+this.Objectuid,-1108,4,4);
                        return -1;
                    }
                }                    
            }
            set {
                if (this.useobjindexer==false) {
                    this.objecttype = value;
                } else {
                    if (this.objindexer.objectlist.IndexOfKey(this.Objectuid)>-1) {
                        this.objindexer.objectlist[this.Objectuid].ObjectType=value;
                    } else {
                        this.proghmi.sendError("BlockContainer -> BlockObject","There wasn't object in ObjectIndexer with such UID! UID:"+this.Objectuid,-1109,4,4);
                    }
                }                     
            }
        }

        private long objectuid;
        /// <summary> Tämä property palauttaa tai settaa itse objektille annetun blockuid:n! HUOM, ei siis tämän luokan instanssin uid:tä, vaan nimenomaan objectin uid:n </summary>
        public long Objectuid {
            get { return this.objectuid;}
            set { this.objectuid = value; }
        }

        /// <summary>
        /// Tieto siitä käytetäänkö ohjelman sisäistä objectindexeriä viitetietojen säilytyspaikkana vai tämän BlockContainerin paikkaa. Lähtökohtaisesti ObjectIndexer olisi oikea paikka tietojen säilytykselle, koska tällöin viitetieto olisi kiinteästi vain yhdessä paikaassa, joka olisi löydettävissä UID:in perusteella
        /// </summary> 
        private bool useobjindexer=false;

        /// <summary>
        /// Referenssi ObjectIndexer luokkaan, jossa säilytetään viitetiedot objekteihin, niiden tyyppeihin ja niiden UID tietoihin.
        /// </summary> 
        private ObjectIndexer objindexer;

        /// <summary>
        /// Käyttöliittymän referenssi
        /// </summary> 
        private ProgramHMI proghmi;

        /// <summary>
        /// Constructor luokalle, joka sitoo alaobjektin sekä objektin tyypin samaan olioon, jotta tältä luokalta voi kysyä suoraan, minkä tyyppinen objekti meillä on tallennettuna
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="blockobj"> object, settaa itse objektin, mutta ei sen tyyppiä </param>
        /// <param name="objtype"> int, settaa itse objektin tyypin, jonka perusteella objekti voidaan castata halaumanlaiseksi ja sitä voidaan käsitellä</param>
        /// <param name="objuid"> long, settaa itse objektille annetun blockuid:n! HUOM, ei siis tämän luokan instanssin uid:tä, vaan nimenomaan objectin uid:n</param>
        /// <param name="prohmi">ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="useobjectindexer"> bool, tieto siitä pidetäänkö objectin tyyppiä ja objectin referenssiä tässä luokassa kirjattuna erillisenä vain kirjattuna vai onko tiedot ObjectIndexerin alla </param>
        /// <param name="objind">ObjectIndexer, objectindexer luokan referenssi, joka säilyttää kaikkien objectien UID tietoja, niiden tyyppejä ja viittauksia objekteihin</param>
        /// <returns>{void}</returns>
        public BlockContainer(string kutsuja, object blockobj, int objtype, long objuid, ProgramHMI prohmi, bool useobjectindexer=false, ObjectIndexer objind=null)
        {
            string functionname="->(BCO)BlockContainer";
            this.objectuid=objuid;
            this.useobjindexer=useobjectindexer;
            this.objindexer=objind;
            this.proghmi=prohmi;
            if (useobjectindexer==true && objind!=null) {
                if (this.objindexer.objectlist.IndexOfKey(objectuid)>-1) {
                    this.objindexer.objectlist[objectuid].ObjectType=objtype;
                    this.objindexer.objectlist[objectuid].ObjectRef=blockobj;
                } else {
                    prohmi.sendError(kutsuja+functionname,"Couldn't add blockobject to objectindexer because of lack of indexer correct UID! Using object own storage capasity instead! UID:"+objuid,-1105,4,4);
                    this.useobjindexer=false;
                    this.blockobject=blockobj;
                    this.objecttype=objtype;
                }
            } else {
                this.blockobject=blockobj;
                this.objecttype=objtype;
            }
        }
    }
    

    /// <summary> Tämä luokka pitää sisällään yksittäiset askelet, jotka sisältävät mahdollisesti vertailuja, tietokahvoja, toiminteita kuten osto ja myynti limit ja market muotoisina 
    /// sekä niistä palautuneita tietoja, mutta myös odotusta niin kauan, että jotain tapahtuu sekä odotukseen liittyviä leikkureita, jotka nollaavat vaiheen, jos esim. odotus on kestänyt liian pitkään </summary> 
    class StepBlock
    {

    }

    /// <summary> Tämä luokka koostaa StepBlockeista kokonaisen toimintaketjun, joka lähtee liikkeelle jostain aloituspisteestä ja loppuu johonkin lopetuspisteeseen </summary>
    class StepMotor
    {

    }