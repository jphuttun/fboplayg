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
    /// Tehdasmetodi, jolla voi luoda erilaisia blokkeja käytettäväksi ja ajettavaksi blokkijärjestelmässä. Tämä luokka toimii myös hallinnoivana luokkana luotujen blokkiobjektien osalta ja tämä luokka on tietoinen luoduista blokeista niiden UID:in perusteella
    /// </summary>
    public class OperationalBlocksFactory
    {

        /// <summary> Käyttöliittymästä vastaavan luokan referenssi </summary>
        private ProgramHMI proghmi;

        /// <summary>
        /// ObjectIndexer, Object instance that give unique UID's to different objects.
        /// </summary>
        private ObjectIndexer objectindexer;

        /// <summary> Lista kaikista blokeista, joissa long edustaa UID arvoa ja BlockContainer edustaa luokkaa, joka sisältää blokin luodun objektin, objektin tyypin numeron, sekä objektin UID:n  </summary>
        private SortedList<long, BlockContainer> listofallblocks;
        /// <summary> Referenssi listaan kaikista blokeista, joissa long edustaa UID arvoa ja BlockContainer edustaa luokkaa, joka sisältää blokin luodun objektin, objektin tyypin numeron, sekä objektin UID:n  </summary>
        public SortedList<long, BlockContainer> ReturnListOfAllBlocks {
            get { return this.listofallblocks; }
        }

        /// <summary>
        /// Tämä Dictionary pitää sisällään kaikkien MainBox komponenttien luomiseen tarvittavat parametrit
        /// </summary>
        private Dictionary<int, RectangleData> rectangledatadict;             

        /// <summary>
        /// Tämä Dictionary pitää sisällään kaikkien MainBox komponenttien luomiseen tarvittavat parametrit ja tällä propertyllä on mahdollista palauttaa kyseisen paremetrilistauksen referenssi
        /// </summary>
        public Dictionary<int, RectangleData> ReturnRectangleDataDictRef {
            get { return this.rectangledatadict; }
        }

        /// <summary>
        /// Constructori tehdasmetodi, jolla voi luoda erilaisia blokkeja käytettäväksi ja ajettavaksi blokkijärjestelmässä. Tämä luokka toimii myös hallinnoivana luokkana luotujen blokkiobjektien osalta ja tämä luokka on tietoinen luoduista blokeista niiden UID:in perusteella
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="prhmi"> ProgramHMI, käyttöliittymä luokan referenssi </param>
        /// <param name="objind">ObjectIndexer, Object instance that give unique UID's to different objects. </param>
        /// <returns> {void} </returns>
        public OperationalBlocksFactory (string kutsuja, ProgramHMI prohmi, ObjectIndexer objind)
        {
            this.proghmi=prohmi;
            this.objectindexer=objind;
            this.listofallblocks = new SortedList<long, BlockContainer>();            
        }

        /// <summary>
        /// Tehdasmetodin kaltainen metodi luomaan listaus luotavien blokkien luontiparametreista
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="rectdatadict"></param> 
        public static void CreateRectangleDataDictionary(string kutsuja, out Dictionary <int, RectangleData> rectdatadict)
        {
            double blockboxwidth=150;
            double blockboxheight=115;
            double blockconnectionwidth=10;

            // Luodaan ohjeet blokkien luomiseksi asettamalla luontia varten olennaiset parametrit listaan
            rectdatadict = new Dictionary<int, RectangleData>
            {
                [(int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_NORMAL_IF_1] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_NORMAL_IF_1, BoxColor = 10, TextColor = 9, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 1, YellowBoxLetters = "I", GreenBoxCount = 1, GreenBoxLetters = "C", RedBoxCount = 3, RedBoxLetters = "RTF" },
                [(int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_BETWEEN_OUTSIDE_2] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.COMPARISON_BLOCK_BETWEEN_OUTSIDE_2, BoxColor = 10, TextColor = 9, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 2, YellowBoxLetters = "HL", GreenBoxCount = 1, GreenBoxLetters = "C", RedBoxCount = 3, RedBoxLetters = "RTF" },
                [(int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100, BoxColor = 12, TextColor = 1, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 0, YellowBoxLetters = "", GreenBoxCount = 0, GreenBoxLetters = "", RedBoxCount = 1, RedBoxLetters = "V" },
                [(int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_200] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_200, BoxColor = 3, TextColor = 1, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 1, YellowBoxLetters = "O", GreenBoxCount = 1, GreenBoxLetters = "I", RedBoxCount = 1, RedBoxLetters = "R" },
                [(int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_3_PROPERTY_201] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.OPERATION_BLOCK_3_PROPERTY_201, BoxColor = 3, TextColor = 1, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 2, YellowBoxLetters = "OO", GreenBoxCount = 1, GreenBoxLetters = "I", RedBoxCount = 1, RedBoxLetters = "R" },
                [(int)ActionCentre.blockTypeEnum.HANDLE_BLOCK_IN_300] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.HANDLE_BLOCK_IN_300, BoxColor = 4, TextColor = 1, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 0, YellowBoxLetters = "", GreenBoxCount = 0, GreenBoxLetters = "", RedBoxCount = 1, RedBoxLetters = "O" },
                [(int)ActionCentre.blockTypeEnum.HANDLE_BLOCK_OUT_301] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.HANDLE_BLOCK_OUT_301, BoxColor = 4, TextColor = 1, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 1, YellowBoxLetters = "I", GreenBoxCount = 0, GreenBoxLetters = "", RedBoxCount = 0, RedBoxLetters = "" },
                [(int)ActionCentre.blockTypeEnum.MARKET_BUY_BLOCK_400] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.MARKET_BUY_BLOCK_400, BoxColor = 11, TextColor = 0, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 1, YellowBoxLetters = "S", GreenBoxCount = 1, GreenBoxLetters = "L", RedBoxCount = 3, RedBoxLetters = "USE" },
                [(int)ActionCentre.blockTypeEnum.MARKET_SELL_BLOCK_401] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.MARKET_SELL_BLOCK_401, BoxColor = 5, TextColor = 1, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 1, YellowBoxLetters = "S", GreenBoxCount = 1, GreenBoxLetters = "L", RedBoxCount = 3, RedBoxLetters = "USE" },
                [(int)ActionCentre.blockTypeEnum.LIMIT_BUY_BLOCK_402] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.LIMIT_BUY_BLOCK_402, BoxColor = 7, TextColor = 0, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 2, YellowBoxLetters = "SC", GreenBoxCount = 1, GreenBoxLetters = "L", RedBoxCount = 3, RedBoxLetters = "USE" },
                [(int)ActionCentre.blockTypeEnum.LIMIT_SELL_BLOCK_403] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.LIMIT_SELL_BLOCK_403, BoxColor = 6, TextColor = 0, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 2, YellowBoxLetters = "SC", GreenBoxCount = 1, GreenBoxLetters = "L", RedBoxCount = 3, RedBoxLetters = "USE" },
                [(int)ActionCentre.blockTypeEnum.TEST_IF_FILLED_BLOCK_500] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.TEST_IF_FILLED_BLOCK_500, BoxColor = 2, TextColor = 0, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 1, YellowBoxLetters = "U", GreenBoxCount = 1, GreenBoxLetters = "L", RedBoxCount = 5, RedBoxLetters = "RTPFE" },
                [(int)ActionCentre.blockTypeEnum.TEST_IF_REMOVED_MOWWM_BLOCK_600] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.TEST_IF_REMOVED_MOWWM_BLOCK_600, BoxColor = 14, TextColor = 1, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 1, YellowBoxLetters = "U", GreenBoxCount = 1, GreenBoxLetters = "L", RedBoxCount = 2, RedBoxLetters = "RTF" },
                [(int)ActionCentre.blockTypeEnum.RESET_ALL_BLOCK_VALUES_700] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.RESET_ALL_BLOCK_VALUES_700, BoxColor = 15, TextColor = 1, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 0, YellowBoxLetters = "", GreenBoxCount = 1, GreenBoxLetters = "C", RedBoxCount = 1, RedBoxLetters = "C" },
                [(int)ActionCentre.blockTypeEnum.END_FOR_NOW_BLOCK_800] = new RectangleData { BoxType = (int)ActionCentre.blockTypeEnum.END_FOR_NOW_BLOCK_800, BoxColor = 16, TextColor = 0, MainBoxWidth = blockboxwidth, MainBoxHeight = blockboxheight, SmallBoxWidth = blockconnectionwidth, YellowBoxCount = 0, YellowBoxLetters = "", GreenBoxCount = 1, GreenBoxLetters = "C", RedBoxCount = 0, RedBoxLetters = "" }
            };
        }

        /// <summary>
        /// Jos tiedämme objektin UID:n, voimme pyytää järjestelmää palauttamaan kohteen objektina. Tämä ei kuitenkaan palauta objektin tyyppiä, vaan sitä on pyydettävä toisella käskyllä
        /// </summary>
        /// <param name="kutsuja"> kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek"> long, UID numero objektille, jota etsitään listofallblocks SortedList listasta. </param>
        /// <returns>{object} palauttaa kohteen tyyppinä object, jos löysi objektin listasta tai null, jos ei löytänyt sitä listasta numerolla UIDtoseek</returns>
        public object ReturnObjectByUID(string kutsuja, long UIDtoseek)
        {
            string functionname="->(OBF)ReturnObjectByUID";
            if (this.listofallblocks.IndexOfKey(UIDtoseek)>-1) {
                return this.listofallblocks[UIDtoseek].BlockObject;
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Didn't find object form listofallblocks with that UID:"+UIDtoseek,-1103,4,4);
                return null;
            }
        }

        /// <summary>
        /// Tämä järjestelmä palauttaa objektin tyypin int numeron UID:n perusteella. Tämä metodi on sukulaismetodi ReturnObjectByUID metodille.
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <returns> {int} palauttaa objektin tyypin numeron UID:n perusteella</returns>
        public int ReturnObjectTypeByUID(string kutsuja, long UIDtoseek)
        {
            string functionname="->(OBF)ReturnObjectTypeByUID";
            int retcode=-1;
            retcode=this.objectindexer.ReturnObjectType(kutsuja+functionname,UIDtoseek);
            if (retcode<0) {
                this.proghmi.sendError(kutsuja+functionname,"Problem to return object type int! UIDtoseek:"+UIDtoseek+" Answer:"+retcode,-1099,4,4);
            }
            return retcode;
        }

        /// <summary>
        /// Etsii uid tiedolla blokkia ja palauttaa IOperationalBlocks &lt; T &gt; tyyppisen objektin BlockHandles luokan referenssin, joka ylläpitää kahvojen tietoja.
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <returns>{BlockHandles} Palauttaa IOperationalBlocks &lt; T &gt; tyyppisen objektin BlockHandles luokan referenssin, joka ylläpitää kahvojen tietoja. </returns>
        public BlockHandles GetBlockHandlesByUID(string kutsuja, long UIDtoseek)
        {
            string functionname = "->(OBF)GetBlockHandlesByUID";
            if (this.listofallblocks.IndexOfKey(UIDtoseek) > -1) {
                // Hakee objektin ObjectIndexeristä
                object obj = this.listofallblocks[UIDtoseek].BlockObject;

                // Yrittää muuntaa objektin IOperationalBlocks-tyyppiin
                if (obj is IOperationalBlocks<object> operationalBlock) {
                    // Palauttaa BlockHandles-viitteen
                    return operationalBlock.ReturnBlockHandlesRef;
                } else {
                    // Käsittele tapaus, jossa tyyppimuunnos epäonnistuu
                    this.proghmi.sendError(kutsuja + functionname, "Object does not implement IOperationalBlocks! UID:"+UIDtoseek, -1300, 4, 4);
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "UID not found in list: " + UIDtoseek, -1301, 4, 4);
            }
            return null;
        }


        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <returns></returns> 
        public bool CheckIncomingHandles(string kutsuja, long UIDtoseek)
        {
            string functionname = "->(OBF)CheckIncomingHandles";
            long parentmotherrectuid=-1;
            if (this.listofallblocks.IndexOfKey(UIDtoseek) > -1)
            {
                int objtype = this.listofallblocks[UIDtoseek].Objecttype;
                object blockobj = this.listofallblocks[UIDtoseek].BlockObject;
                if (this.objectindexer.objectlist.IndexOfKey(UIDtoseek)>-1) {
                    parentmotherrectuid=this.objectindexer.objectlist[UIDtoseek].ParentUID;

                    if (blockobj != null) {
                        switch (objtype)
                        {
                            case (int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_COMPARISON_OBJECT_340:
                                ComparisonBlock comparisonBlock = this.objectindexer.GetTypedObject<ComparisonBlock>(kutsuja+functionname,UIDtoseek);
                                if (comparisonBlock != null)
                                {
                                    comparisonBlock.CheckIncomingHandles(kutsuja+functionname,UIDtoseek,parentmotherrectuid);
                                } else {
                                    this.proghmi.sendError(kutsuja + functionname, "Failed to cast to ComparisonBlock! UID:"+UIDtoseek+" ErrorCode:"+this.objectindexer.GetLastError.ErrorCode+" ErrorMessage:"+this.objectindexer.GetLastError.WholeErrorMessage, -1116, 4, 4); // Log error or handle case where cast fails
                                }
                                break;
                            case (int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_OPERATIONBLOCK_OBJECT_341:
                                OperationBlock operationBlock = this.objectindexer.GetTypedObject<OperationBlock>(kutsuja+functionname,UIDtoseek);
                                if (operationBlock != null)
                                {
                                    operationBlock.CheckIncomingHandles(kutsuja+functionname,UIDtoseek,);
                                } else {
                                    this.proghmi.sendError(kutsuja + functionname, "Failed to cast to OperationalBlock! UID:"+UIDtoseek+" ErrorCode:"+this.objectindexer.GetLastError.ErrorCode+" ErrorMessage:"+this.objectindexer.GetLastError.WholeErrorMessage, -1117, 4, 4); // Log error or handle case where cast fails
                                }
                                break;
                            default:
                                this.proghmi.sendError(kutsuja + functionname, "Invalid object type", -1118, 4, 4);
                                break;
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Block object reference was null and it wasn't possible to check incoming handles states! UID:" + UIDtoseek, -1114, 4, 4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Block parent wasn't set! Block UID:" + UIDtoseek, -1183, 4, 4);
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "No object found with UID to seek! UID:" + UIDtoseek, -1115, 4, 4);
            }
            
        }      
        
        /// <summary>
        /// Tehdasmetodi, jolla luodaan IOperationalBlocks tyyppisiä blokkiobjekteja, jotka täyttävät rajapinnan IOperationalBlocks määreet
        /// </summary>
        /// <typeparam name="T">ComparisonBlock, OperationBlock = parametri, jonka tyyppisiä kohteita tehdasmetodilla voidaan tehdä </typeparam>
        /// <param name="caller"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="hmireference"> ProgramHMI, Käyttöliittymän referenssi </param>
        /// <param name="objind">ObjectIndexer, referenssi ObjectIndexer luokkaan joka ylläpitää tietoja minkä tyyppisistä objekteista on kyse ja niiden UID tiedoista sekä objektin instanssin referenssistä </param>
        /// <param name="objtype">int, itse kohteen objektin tyyppi - ks. vaihtoehdot enumeraatio ObjectIndexer.indexerObjectTypes </param>
        /// <param name="parentuid">long, vanhemman UID </param>
        /// <param name="granparentuid">long, isovanhemman UID </param>
        /// <param name="parentobjtype">int, kohteen vanhemman objektin tyyppi - ks. vaihtoehdot enumeraatio ObjectIndexer.indexerObjectTypes </param>
        /// <param name="altroute"></param>
        /// <param name="blockname"></param>
        /// <param name="parentobj"></param>
        /// <param name="obfparams">OperationalBlocksFactoryParams luokan instanssin referenssi, joka pitää sisällään parametrit, joita ollaan asettamassa nimenomaisesti kyseiselle luotavalle luokalle ja sen instanssille</param>
        /// <returns> {BlockContainer}, palauttaa BlockContainer tyyppisen luokan instanssin referenssin, joka sisältää objektin tyypin, objektin referenssin ja objektin UID tiedon </returns>
        /// <exception cref="InvalidOperationException">Tämä exception tulee jos on yritetty luoda luokkaa, joka ei ole yksi OperationalBlocks rajapinnan luokista</exception>
        public BlockContainer CreateOperationalBlock<T>(string caller, ProgramHMI hmireference, ObjectIndexer objind, int objtype, long parentuid, long granparentuid, int parentobjtype, int altroute, string blockname, object parentobj, OperationalBlocksFactoryParams obfparams) where T : notnull
        {
            long objectuid=-1;
            object newBlock = null;
            BlockContainer blockContainer= null;
            int objindtype=-1;
            int response=-1;

            string functionname="->(OBF)CreateOperationalBlock<type_of_block>";
            // Voit lisätä logiikkaa tehdasmetodiin, jos haluat päättää dynaamisesti,
            // mikä aliluokan instanssi luodaan perustuen esimerkiksi `T`:n tyyppiin.
            if (typeof(T) == typeof(ComparisonBlock)) {
                objindtype=objtype;
                objectuid=objind.AddObjectToIndexer(caller+functionname,parentuid,objindtype,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,granparentuid);
                newBlock = (IOperationalBlocks<T>)(object)new ComparisonBlock(caller+functionname,hmireference,objind,objectuid, parentuid, granparentuid, obfparams.ComparisonBlockType,obfparams.SelectedHandle,altroute,blockname); //
                response=objind.SetObjectToIndexerWithErrorReport(caller+functionname,objectuid,newBlock); // Asetetaan juuri luodun objektin referenssi vielä objectindexeriin
                if (response<0) {
                    this.proghmi.sendError(caller+functionname,"Couldn't set created object to object indexer! UID:"+objectuid+" Response:"+response,-1111,4,4);
                } else {
                    blockContainer = new BlockContainer(caller+functionname,newBlock,objindtype,objectuid,hmireference,true,objind);
                    this.listofallblocks.Add(objectuid, blockContainer); // Lisätään luotu blockContainer blokkilistalle
                }
                //return (IOperationalBlocks<T>)(object)new ComparisonBlock(kutsuja+functionname,hmireference); 
            } else if (typeof(T) == typeof(OperationBlock)) {
                objindtype=objtype;
                objectuid=objind.AddObjectToIndexer(caller+functionname,parentuid,objindtype,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,granparentuid);
                newBlock = (IOperationalBlocks<T>)(object)new OperationBlock(caller+functionname,hmireference,objind, objectuid, parentuid, granparentuid, obfparams.OperationalBlockType,obfparams.SelectedHandle,altroute,blockname); //
                response=objind.SetObjectToIndexerWithErrorReport(caller+functionname,objectuid,newBlock); // Asetetaan juuri luodun objektin referenssi vielä objectindexeriin
                if (response<0) {
                    this.proghmi.sendError(caller+functionname,"Couldn't set created object to object indexer! UID:"+objectuid+" Response:"+response,-1112,4,4);
                } else {
                    blockContainer = new BlockContainer(caller+functionname,newBlock,objindtype,objectuid,hmireference,true,objind);
                    this.listofallblocks.Add(objectuid, blockContainer); // Lisätään luotu blockContainer blokkilistalle
                }                
                //return (IOperationalBlocks<T>)(object)new OperationBlock(kutsuja+functionname,hmireference);               
            } else {
                // Heitä poikkeus tai käsittele tilanne, jos T ei ole tuettu tyyppi
                throw new InvalidOperationException("Unsupported type parameter for OperationalBlocks.");
            }
            return blockContainer;
        }
    }     

    /// <summary>
    /// Tämä luokka pitää sisällään parametrit, joita on mahdollista antaa CreateOperationalBlock tehdasmetodille
    /// </summary> 
    public class OperationalBlocksFactoryParams
    {
        private int comparisonblocktype=-1;
        /// <summary> int, Minkä tyyppinen (esim. comparison block) on kyseessä. 1=normaali valinta, 2=between/outside tyyppinen tarkastelut </summary>
        public int ComparisonBlockType {
            get { return this.comparisonblocktype; }
            set { this.comparisonblocktype=value; }
        }

        private int operationalblocktype=-1;
        /// <summary> Kuinka monen erillisen muuttujan matemaattinen blokki on kyseessä. 2=kahden muuttujan (1+1), 3=kolmen muuttujan (2+1) </summary>
        public int OperationalBlockType {
            get { return this.operationalblocktype; }
            set { this.operationalblocktype=value; }
        }

        /// <summary>
        /// Minkälainen blokki on kyseessä - onko kyseessä blokki, jolle käyttäjä antaa omat arvonsa vai blokki, jossa haetaan arvo parametrilistasta vai minkätyyppinen value block
        /// </summary>
        private int valueblocktype;
        /// <summary>
        /// Minkälainen blokki on kyseessä - onko kyseessä blokki, jolle käyttäjä antaa omat arvonsa vai blokki, jossa haetaan arvo parametrilistasta vai minkätyyppinen value block
        /// </summary>
        public int ValueBlockType {
            get { return this.valueblocktype; }
        }        

        private int selectedhandle=-1;
        /// <summary> int, Se vaihtoehto, joka on valittu comparisonblock kohteessa Operator comboboxin valinnaksi </summary>
        public int SelectedHandle {
            get { return this.selectedhandle; }
            set { this.selectedhandle=value; }
        }
    }

    /// <summary>
    /// Tämä tietorakenne luokka pitää sisällään tiedot, jonka perusteella voidaan luoda erilaisia mainbox komponentteja ruudulle
    /// </summary>
    public class RectangleData
    {
        /// <summary>
        /// The type of the box.
        /// </summary>
        public int BoxType { get; set; }

        /// <summary>
        /// The color index of the main box.
        /// </summary>
        public int BoxColor { get; set; }

        /// <summary>
        /// The color index of the text.
        /// </summary>
        public int TextColor { get; set; }

        /// <summary>
        /// The width of the main box.
        /// </summary>
        public double MainBoxWidth { get; set; }

        /// <summary>
        /// The height of the main box.
        /// </summary>
        public double MainBoxHeight { get; set; }

        /// <summary>
        /// The width of the small boxes on the side of the main box.
        /// </summary>
        public double SmallBoxWidth { get; set; }

        /// <summary>
        /// The number of yellow boxes.
        /// </summary>
        public int YellowBoxCount { get; set; }

        /// <summary>
        /// The letters to be printed in the yellow boxes.
        /// </summary>
        public string YellowBoxLetters { get; set; }

        /// <summary>
        /// The number of green boxes.
        /// </summary>
        public int GreenBoxCount { get; set; }

        /// <summary>
        /// The letters to be printed in the green boxes.
        /// </summary>
        public string GreenBoxLetters { get; set; }

        /// <summary>
        /// The number of red boxes.
        /// </summary>
        public int RedBoxCount { get; set; }

        /// <summary>
        /// The letters to be printed in the red boxes.
        /// </summary>
        public string RedBoxLetters { get; set; }
    }    

    /// <summary>
    ///  Tämän luokan instanssi pitää sisällään yhden arvon sekä tyypin, mikä arvo on kyseessä. Näillä objekteilla ei toistaiseksi ole omaa UID numeroa, eikä parentin uid tietoa
    /// </summary>
    public class BlockAtomValue
    {
        /// <summary>
        /// Enumeraatio, joka määrittelee atomityypin.
        /// </summary>
        public enum AtomTypeEnum {
            Not_defined_yet = 0,
            Int = 1,
            Long = 2,
            Decimal = 3,
            String = 4,
            Bool = 5,
            /// <summary>
            /// Sama kokonaisluku on asetettu BlockAtomValue:ssa sekä Long että Int arvoiksi
            /// </summary>
            LONG_AND_INT = 100,
            /// <summary>
            /// Sama kokonaisluku on asetettu BlockAtomValue:ssa sekä Long, Decimal että Int arvoiksi
            /// </summary>            
            LONG_AND_INT_AND_DEC = 101,
            /// <summary>
            /// Sama kokonaisluku on asetettu BlockAtomValue:ssa sekä Long, Bool että Int arvoiksi. Tällöin hyväksytään vain luvut 1 ja 0, joista 1=true ja 0=false
            /// </summary>            
            LONG_AND_INT_AND_BOOL = 102,
            /// <summary>
            /// Sama kokonaisluku on asetettu BlockAtomValue:ssa sekä Long, Bool että Int arvoiksi. Tällöin hyväksytään vain luvut 1 ja 0, joista 1=true ja 0=false. True tai false kirjoitetaan myö string muuttujaan. Tämä luku 0 tai 1 asetetaan myös desimaaliluvuksi atomiin.
            /// </summary>            
            LONG_AND_INT_AND_DEC_AND_STRING_AND_BOOL=105,
            /// <summary>
            /// Int arvo, joka on myös asetettuna string muotoisena atomin string muuttujaan
            /// </summary>
            STRING_AND_INT = 200,
            /// <summary>
            /// Long arvo, joka on myös asetettuna string muotoisena atomin string muuttujaan
            /// </summary>            
            STRING_AND_LONG = 201,
            /// <summary>
            /// Decimal arvo, joka on myös asetettuna string muotoisena atomin string muuttujaan
            /// </summary>            
            STRING_AND_DEC = 202,
            /// <summary>
            /// Bool arvo, joka on myös asetettuna string muotoisena atomin string muuttujaan. Jos Bool arvo on true, on string muotoiseen muuttujaan kirjoitettu "true" ja jos bool arvo on false on string muotoiseen muuttujaan kirjoitettu "false"
            /// </summary>            
            STRING_AND_BOOL = 203
        }

        private int intatom=-1;

        public int IntAtom { 
            get { return intatom; } 
            set {
                this.atomtype=(int)AtomTypeEnum.Int;
                this.intatom=value;
            } 
        }

        private int intatomminusdiff=0;

        public int IntAtomMinusDiff {
            get { return this.intatomminusdiff; }
            set { this.intatomminusdiff=value; }
        }

        private int intatomplusdiff=0;
        public int IntAtomPlusDiff {
            get { return this.intatomplusdiff; }
            set { this.intatomplusdiff=value; }
        }        

        private long longatom=-1;
        public long LongAtom { 
            get { return this.longatom; }
            set {
                this.atomtype=(int)AtomTypeEnum.Long;
                this.longatom=value;
            }
        }

        private long longatomminusdiff = 0;
        public long LongAtomMinusDiff {
            get { return this.longatomminusdiff; }
            set { this.longatomminusdiff = value; }
        }

        private long longatomplusdiff = 0;
        public long LongAtomPlusDiff {
            get { return this.longatomplusdiff; }
            set { this.longatomplusdiff = value; }
        }

        private decimal decatom=-1;
        public decimal DecAtom { 
            get { return this.decatom; }
            set {
                this.atomtype=(int)AtomTypeEnum.Decimal;
                this.decatom=value;
            }
        }

        private decimal decatomminusdiff = 0;
        public decimal DecAtomMinusDiff {
            get { return this.decatomminusdiff; }
            set { this.decatomminusdiff = value; }
        }

        private decimal decatomplusdiff = 0;
        public decimal DecAtomPlusDiff {
            get { return this.decatomplusdiff; }
            set { this.decatomplusdiff = value; }
        }      

        private string stringatom="";
        public string StringAtom { 
            get { return stringatom; } 
            set {
                this.atomtype=(int)AtomTypeEnum.String;
                this.stringatom=value; 
            } 
        }
        private string stringatomminusdiff = "";
        public string StringAtomMinusDiff {
            get { return this.stringatomminusdiff; }
            set { this.stringatomminusdiff = value; }
        }

        private string stringatomplusdiff = "";
        public string StringAtomPlusDiff {
            get { return this.stringatomplusdiff; }
            set { this.stringatomplusdiff = value; }
        }       

        private bool boolatom=false;

        public bool BoolAtom {
            get { return boolatom; }
            set {
                this.atomtype=(int)AtomTypeEnum.Bool;
                this.boolatom=value;
            }
        }

        private bool boolatomminusdiff = false;
        public bool BoolAtomMinusDiff {
            get { return this.boolatomminusdiff; }
            set { this.boolatomminusdiff = value; }
        }

        private bool boolatomplusdiff = false;
        public bool BoolAtomPlusDiff {
            get { return this.boolatomplusdiff; }
            set { this.boolatomplusdiff = value; }
        }         

        private int atomtype=(int)AtomTypeEnum.Not_defined_yet;
        /// <summary>
        /// AtomType kertoo, minkä tyyppinen kohde on kyseessä AtomTypeEnum enumeraatiota. Jos haluat jonkin monimutkaisemman BlockAtomType:n säilytysmuodon, niin aseta se ensin tähän AtomType muuttujaan ja sitten vasta itse luku halutussa muodossa
        /// </summary>
        public int AtomType { 
            get { return this.atomtype; }
            set { this.atomtype=value; }
        }

        /// <summary>
        /// Tämä metodi asettaa atomiin kerralla useita parametreja eri muodoissaan
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="atomtypeval">int enum, atomin tyyppi AtomTypeEnum enumeraation mukaan</param>
        /// <param name="intval"></param>
        /// <param name="longval"></param>
        /// <param name="decval"></param>
        /// <param name="strval"></param>
        /// <param name="boolval"></param>
        public void AtomMultiSet(string kutsuja, int atomtypeval, int intval=-1, long longval=-1, decimal decval=-1, string strval="", bool boolval=false )
        {
            this.atomtype=atomtypeval;
            this.intatom=intval;
            this.longatom=longval;
            this.decatom=decval;
            this.stringatom=strval;
            this.boolatom=boolval;
        }

        public BlockAtomValue()
        {
            this.Clear();
        }

        /// <summary>
        /// Tällä metodilla tyhjätään BlockAtomValue luokan instanssi
        /// </summary>
        /// <returns> {void} </returns>
        public void Clear() 
        {
            this.atomtype=(int)AtomTypeEnum.Not_defined_yet;
            this.intatom=-1;
            this.longatom=-1;
            this.decatom=-1;
            this.stringatom="";
            this.boolatom=false;
        }

        /// <summary>
        /// Tällä metodi kopioi otherblockatomvalue BlockAtomValue:n instanssin tiedot tähän BlockAtomValuee:n
        /// </summary>
        /// <param name="otherblockatomvalue"> BlockAtomValue, otherblockatomvalue BlockAtomValue:n instanssin tiedot jotka kopioidaan tähän BlockAtomValue:en </param>
        /// <returns> {void} </returns>
        public void CopyFrom(BlockAtomValue otherblockatomvalue)
        {
            int contempo=otherblockatomvalue.AtomType;
            this.intatom=otherblockatomvalue.IntAtom;
            this.longatom=otherblockatomvalue.LongAtom;
            this.decatom=otherblockatomvalue.DecAtom;
            this.stringatom=otherblockatomvalue.StringAtom;
            this.boolatom=otherblockatomvalue.BoolAtom;
            this.atomtype=contempo;
        }
    }

    /// <summary>
    /// Tämä luokka pitää sisällään CheckIncomingHandles käskyn palautuksessa listan niitä UID arvoja, jota EIVÄT ole vielä päivittyneet tietojensa tietojensa osalta 
    /// </summary>
    public class IncomingHandlesStatus
    {
        /// <summary>
        /// Käyttöliittymän referenssi
        /// </summary> 
        private ProgramHMI proghmi;

        /// <summary> Yksittäisen IncomingHandlesStatus objektin instanssin oma uniqrefnum eli UID </summary>
        public long OwnUID { get; set; }

        /// <summary>
        /// Tämän IncomingHandlesStatus vanhemman UID
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Tämän IncomingHandlesStatus isovanhemman UID
        /// </summary>
        public long GranParentUID { get; set; }

        /// <summary>
        /// Oletus, että incominghandle laatikoita on blokilla, mutta kaikilla niitä ei ole kuten esim. ValueBlockilla
        /// </summary> 
        private bool noincominghandles=false;

        /// <summary>
        /// Tämä enumeraatio kertoo, onko palautettava tieto True=1, jolloin lista on tyhjä ja voi ajaa ExecuteBlock käskyä, vai False=0, jolloin listalla on kohteita, jotka eivät ole saaneet vielä arvoa, vai -1, jolloin kohteessa ei ole ollenkaan olemassa Incoming laatikoita lainkaan
        /// </summary> 
        public enum handleStatusOkay {
            /// <summary>
            /// Kohteella ei ole ylipäätään olemassa incominghandle laatikoita (keltaiset ja vihreät)
            /// </summary>
            HANDLE_STATUS_WITH_NO_INCOMING_HANDLES_MINUS_1=-1,
            /// <summary>
            /// Jokin tulopuolen kahvoista ei ole vielä saanut arvoa itselleen
            /// </summary>
            HANDLE_STATUS_FALSE_0=0,
            /// <summary>
            /// Kaikki tulopuolen kahvat ovat saaneet perityn arvon ja voidaan jatkaa ExecuteBlock vaiheeseen
            /// </summary> 
            HANDLE_STATUS_TRUE_1=1
        };

        /// <summary>
        /// Tämä property kertoo, onko vielä jäljellä olevia Connection kohteita, jotka eivät ole saaneet arvoa itselleen ja näinollen estävät ExecuteBlock käskyn ajamisen.
        /// Lisäksi tämä tavallaan myös kertoo, onko tämä luokka valmis käyttöä varten - eli onko listoille jo lisätty jotain, vai onko lista tyhjä. Jos se on tyhjä, niin siihen voi alkaa lisätä uusia kohteita. 
        /// </summary> 
        public int IsIncomingHandleStatusOkay {
            get {
                if (noincominghandles==true) {
                    if (this.listofconnectionuids.Count==0) {
                        return (int)handleStatusOkay.HANDLE_STATUS_TRUE_1;
                    } else {
                        return (int)handleStatusOkay.HANDLE_STATUS_FALSE_0;
                    }
                } else {
                    return (int)handleStatusOkay.HANDLE_STATUS_WITH_NO_INCOMING_HANDLES_MINUS_1;
                }
            }
        }

        /// <summary>
        /// Tässä listassa on kaikki ne Connection tyyppiset kohteet (niiden UID:t), jotka EIVÄT ole saaneet mitään arvoa ja ExecuteBlock käskyä ei tällöin voida ajaa.
        /// Ensimmäinen long on Connection:in instanssin UID. Jälkimmäinen long = -1, jos mitään liite objektin UID ei ole kiinnitetty. Muussa tapauksessa kyseisen liiteobjektin (positiivinen) UID numero ja näin alussa, esim. ConnectioRectangle:n UID, johon kohde liittyy
        /// </summary> 
        private SortedList<long, long> listofconnectionuids;
        
        /// <summary>
        /// Tässä listassa on kaikki ne Connection tyyppiset kohteet (niiden UID:t), jotka eivät ole saaneet mitään arvoa ja ExecuteBlock käskyä ei tällöin voida ajaa
        /// </summary> 
        public SortedList<long, long> ReturnListOfConnectionUIDsReference {
            get { return this.listofconnectionuids; }
        }

        /// <summary>
        /// Tässä listassa on kaikki ne Connection tyyppiset kohteet (niiden UID:t), jotka ovat saaneet jonkin arvon ja ExecuteBlock käskyä voidaan ajaa, jos kaikki kahvat ovat saaneet arvon.
        /// Ensimmäinen long on Connection:in instanssin UID. Jälkimmäinen long = -1, jos mitään liite objektin UID ei ole kiinnitetty. Muussa tapauksessa kyseisen liiteobjektin (positiivinen) UID numero ja näin alussa, esim. ConnectioRectangle:n UID, johon kohde liittyy
        /// </summary> 
        private SortedList<long, long> listofvalidconnectionuids;

        /// <summary>
        /// Tässä listassa on kaikki ne Connection tyyppiset kohteet (niiden UID:t), jotka ovat saaneet arvot
        /// </summary> 
        public SortedList<long, long> ReturnListOfValidConnectionUIDsReference {
            get { return this.listofvalidconnectionuids; }
        }        

        /// <summary>
        /// listofconnectionuids SortedList listalla oleva indeksinumero (se indeksi, joka saadaan IndexOfKey käskyllä). Jos -3, niin epämääräinen indeksinumero, jos -2 = niin ei yhtään kohdetta listalla ja jos -1 = niin indeksi yli viimeisen elementin 
        /// </summary> 
        private int innerindex=-3;

        /// <summary>
        /// listofvalidconnectionuids SortedList listalla oleva indeksinumero (se indeksi, joka saadaan IndexOfKey käskyllä). Jos -3, niin epämääräinen indeksinumero, jos -2 = niin ei yhtään kohdetta listalla ja jos -1 = niin indeksi yli viimeisen elementin 
        /// </summary> 
        private int validinnerindex=-3;        

        /// <summary>
        /// Referenssi ObjectIndexeriin, joka pitää ylhäällä, mitä objekteja olemme luoneet ohjelmaan, joiden ID:t täytyy olla rekisteröitynä
        /// </summary>
        private ObjectIndexer objectIndexer;

        /// <summary>
        /// Tämän luokan instanssi pitää kirjaa, onko luokka initialisoitu oikein
        /// </summary> 
        private InitializationCounterClass classini;

        /// <summary>
        /// Arvo, jonka initialisaatio täytyy ylittää, jotta luokka katsotaan oikein initialisoiduksi.
        /// </summary> 
        private int mustexceedtreshold=0;

        /// <summary>
        /// Tässä listassa on kaikki ne Connection tyyppiset kohteet (niiden UID:t), jotka eivät ole saaneet mitään arvoa ja ExecuteBlock käskyä ei tällöin voida ajaa
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="prohmi"> ProgramHMI, käyttöliittymäluokan referenssi </param>
        /// <param name="objind"> ObjectIndexer luokan referenssi - kyseinen luokka pitää ylhäällä, mitä objekteja olemme luoneet ohjelmaan, joiden ID:t täytyy olla rekisteröitynä </param>
        /// <param name="parentuid"> long, Tämän IncomingHandlesStatusin vanhemman UID </param>
        /// <param name="granparentuid"> long, Tämän IncomingHandlesStatusin isovanhemman UID </param>
        /// <param name="noincominghandleblocks"> bool, jos tämä on luotaessa false, niin tällöin tämä objekti toimii normaalisti ja jos taas true, niin tällöin IncomingHandle:ja ei ole olemassa ylipäätään </param>/// 
        /// <returns> {void} </returns> 
        public IncomingHandlesStatus(string kutsuja, ProgramHMI prohmi, ObjectIndexer objind, long parentuid, long granparentuid, bool noincominghandleblocks=false)
        {
            string functionname="->(IHS)IncomingHandlesStatus";
            this.objectIndexer=objind;
            this.noincominghandles=noincominghandleblocks;
            this.proghmi=prohmi;
            this.listofconnectionuids=new SortedList<long, long>();
            this.listofconnectionuids.Clear();
            this.listofvalidconnectionuids=new SortedList<long, long>();
            this.listofvalidconnectionuids.Clear();
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.classini=new InitializationCounterClass(kutsuja+functionname,this.mustexceedtreshold);

            this.OwnUID=this.objectIndexer.AddObjectToIndexer(kutsuja+functionname,parentuid,(int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_INCOMING_HANDLE_STATUS_339,-1,(int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1,granparentuid);
            if (this.OwnUID>=0) {
                this.classini.AddClassOkayByNumber(kutsuja+functionname,1);
                if (this.objectIndexer.objectlist.IndexOfKey(this.OwnUID)>-1) {
                    int respo=this.objectIndexer.SetObjectToIndexerWithErrorReport(kutsuja+functionname,this.OwnUID,this,(int)ObjectIndexer.rewriteOldObjectReference.ALWAYS_REWRITE_OBJECT_REFERENCE_0);
                    if (respo>=0) {
                        this.classini.AddClassOkayByNumber(kutsuja+functionname,1);
                    } else {
                        this.proghmi.sendError(kutsuja+functionname, "Failed to set object to indexer", -1278, 4, 4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname, "UID not found in object indexer list", -1279, 4, 4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname, "Failed to add object to indexer", -1280, 4, 4);
            }
        }        

        /// <summary>
        /// Tyhjätään kokonaisuudessaan listofconnectionuids
        /// </summary>
        /// <param name="validconnections">int, jos 0, niin tyhjää listofconnecitonuids ja jos 1, niin tyhjää listofvalidconnectionuids ja jos jotain muuta, niin tyhjää molemmat </param>
        /// <returns> {void} </returns>
        public void ClearListOfConnectionUIDs(int validconnections=0)
        {
            if (validconnections==0) {
                this.listofconnectionuids.Clear();
            } else if (validconnections==1) {
                this.listofvalidconnectionuids.Clear();
            } else {
                this.listofconnectionuids.Clear();
                this.listofvalidconnectionuids.Clear();
            }
        }

        /// <summary>
        /// Milloin raportoidaan virheet listofconnectionuids listaan liittyen
        /// </summary> 
        public enum reportingType {
            /// <summary>
            /// Virheita ei raportoida missään tapauksista
            /// </summary> 
            REPORT_ERROR_NEVER = -1,
            /// <summary>
            /// Raportoidaan aina
            /// </summary> 
            REPORT_ERROR_ALWAYS = 0,
            /// <summary>
            /// Raportoidaan vain silloin, kun listalla on jo saman numeroinen kohde ja samalla UID:lla yritetään lisätä uutta
            /// </summary> 
            REPORT_IF_CONNECTION_WITH_SAME_UID = 1,
            /// <summary>
            /// Raportoidaan vain silloin, kun listalta ollaan poistamassa kohdetta ja annetulla UID:lla listalla ei ole yhtään kohdetta
            /// </summary> 
            REPORT_IF_NO_CONNECTION_WITH_SUCH_UID = 2
        };

        /// <summary>
        /// Lisää uuden UID:n listofconnectionuids-listaan.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="UIDtoadd">Lisättävä Connection objektin UID.</param>
        /// <param name="validconnections">bool, tämä määrittää käytetäänkö kumpaa listaa: listofvalidconnectionuids vai listofconnectionuids. Jos true, niin listofvalidconnectionuids johon on listattu kaikki connection uid:it, jotka ovat saaneet arvonsa </param>
        /// <param name="reportingtype">Raportointityyppi, määrittää milloin virheet raportoidaan.</param>
        /// <returns>Palauttaa 1, jos toimenpide onnistui. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos toimenpide epäonnistui.</returns>
        public int AddNewUID(string kutsuja, long UIDtoadd, bool validconnections=false, int reportingtype=(int)reportingType.REPORT_ERROR_ALWAYS)
        {
            string functionname="->(IHS)AddNewUID";
            if (this.classini.IsClassInitialized==true) {
                if (validconnections==false) {
                    return this.AddNewConnectionUID(kutsuja+functionname,UIDtoadd,this.listofconnectionuids, reportingtype);
                } else {
                    return this.AddNewConnectionUID(kutsuja+functionname,UIDtoadd,this.listofvalidconnectionuids, reportingtype);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname, "Class not initialized", -1281, 4, 4);
                return -22;
            }
        }

        /// <summary>
        /// Lisää uuden UID:n listofconnectionuids-listaan.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="UIDtoadd">Lisättävä Connection objektin UID.</param>
        /// <param name="connectionslist">Sortedlist &lt; long, long &gt; , tämä määrittää käytetäänkö kumpaa listaa: listofvalidconnectionuids vai listofconnectionuids. Jos true, niin listofvalidconnectionuids johon on listattu kaikki connection uid:it, jotka ovat saaneet arvonsa </param>
        /// <param name="reportingtype">Raportointityyppi, määrittää milloin virheet raportoidaan.</param>
        /// <returns>Palauttaa 1, jos toimenpide onnistui. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos toimenpide epäonnistui.</returns>
        private int AddNewConnectionUID(string kutsuja, long UIDtoadd, SortedList<long, long> connectionslist, int reportingtype)
        {
            string functionname="->(IHS)AddNewConnectionUID";
            if (connectionslist!=null) {
                if (connectionslist.ContainsKey(UIDtoadd)) {
                    if (reportingtype == (int)reportingType.REPORT_ERROR_ALWAYS || reportingtype == (int)reportingType.REPORT_IF_CONNECTION_WITH_SAME_UID) {
                        this.proghmi.sendError(kutsuja+functionname, "UID already exists in the list: " + UIDtoadd, -1260, 4, 4);
                    }
                    return -20;
                }

                try {
                    Connection conn = this.objectIndexer.GetTypedObject<Connection>(kutsuja+functionname, UIDtoadd);
                    if (conn!=null) {
                        if (conn.ReturnSendingBlockAtomValueRef!=null) {
                            long connendpoint=conn.Box2OwnUID; // Päätepiste
                            if (connendpoint>=0) {
                                ConnectionRectangle connrect = this.objectIndexer.GetTypedObject<ConnectionRectangle>(kutsuja+functionname,connendpoint);
                                if (connrect!=null) {
                                    if (connrect.BlockAtomValueRef!=null) {
                                        connrect.BlockAtomValueRef.CopyFrom(conn.ReturnSendingBlockAtomValueRef);
                                        connectionslist.Add(UIDtoadd, connendpoint); // Lisätään Connectionin UID ja connectionin nuolen päättyvä ConnectioRectanglen uid
                                        return 1;
                                    } else {
                                        this.proghmi.sendError(kutsuja + functionname, "BlockAtomValueRef is null in ConnectionRectangle", -1288, 4, 4);
                                        return -27;
                                    }
                                } else {
                                    this.proghmi.sendError(kutsuja + functionname, "ConnectionRectangle is null for endpoint UID: " + connendpoint, -1289, 4, 4);
                                    return -26;
                                }
                            } else {
                                this.proghmi.sendError(kutsuja + functionname, "Invalid endpoint UID: " + connendpoint, -1290, 4, 4);
                                return -25;
                            }
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "ReturnSendingBlockAtomValueRef is null in Connection - UID:"+UIDtoadd, -1291, 4, 4);
                            return -28;
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Connection is null for UID: " + UIDtoadd, -1292, 4, 4);
                        return -24;
                    }
                } catch (Exception ex) {
                    this.proghmi.sendError(kutsuja + functionname, "Exception: " + ex.Message, -1261, 4, 4);
                    return -21;
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "Connections list is null", -1293, 4, 4);
                return -23;
            }
        }

        /// <summary>
        /// Poistaa UID:n listofconnectionuids- tai listofvalidconnectionuids-listasta.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="UIDtoremove">Poistettava Connection objektin UID.</param>
        /// <param name="validconnections">bool, tämä määrittää käytetäänkö kumpaa listaa: listofvalidconnectionuids vai listofconnectionuids.</param>
        /// <param name="reportingtype">Raportointityyppi, määrittää milloin virheet raportoidaan.</param>
        /// <returns>Palauttaa 1, jos toimenpide onnistui. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos toimenpide epäonnistui.</returns>
        public int RemoveUID(string kutsuja, long UIDtoremove, bool validconnections=false, int reportingtype=(int)reportingType.REPORT_ERROR_ALWAYS)
        {
            string functionname="->(IHS)RemoveUID";
            if (this.classini.IsClassInitialized==true) {
                SortedList<long, long> connectionslist = validconnections ? this.listofvalidconnectionuids : this.listofconnectionuids;

                if (!connectionslist.ContainsKey(UIDtoremove)) {
                    if (reportingtype == (int)reportingType.REPORT_ERROR_ALWAYS || reportingtype == (int)reportingType.REPORT_IF_NO_CONNECTION_WITH_SUCH_UID) {
                        this.proghmi.sendError(kutsuja+functionname, "UID not found in the list: " + UIDtoremove, -1262, 4, 4);
                    }
                    return -31;
                }

                try {
                    connectionslist.Remove(UIDtoremove);
                    return 1;
                } catch (Exception ex) {
                    this.proghmi.sendError(kutsuja+functionname, "Exception: " + ex.Message, -1263, 4, 4);
                    return -32;
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname, "Class not initialized", -1282, 4, 4);
                return -33;
            }
        }

        /// <summary>
        /// Palauttaa ensimmäisen UID:n listofconnectionuids- tai listofvalidconnectionuids-listalta.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="validconnections">bool, tämä määrittää käytetäänkö kumpaa listaa: listofvalidconnectionuids vai listofconnectionuids.</param>
        /// <returns>Palauttaa ensimmäisen UID:n, jos listalla on kohteita. Jos ei ole yhtään kohdetta, palauttaa -2.</returns>
        public long IterateThroughReturnFirst(string kutsuja, bool validconnections=false)
        {
            string functionname="->(IHS)IterateThroughReturnFirst";
            if (this.classini.IsClassInitialized==true) {
                SortedList<long, long> connectionslist = validconnections ? this.listofvalidconnectionuids : this.listofconnectionuids;

                if (connectionslist.Count == 0) {
                    return -2; // Ei yhtään kohdetta listalla
                }

                this.validinnerindex = 0;
                return connectionslist.Keys[this.validinnerindex];
            } else {
                this.proghmi.sendError(kutsuja+functionname, "Class not initialized", -1283, 4, 4);
                return -3;
            }
        }

        /// <summary>
        /// Antaa seuraavan Connection instanssin UID:n listalta käyttäen innerindex laskuria tai annetusta nexttothisUID:sta seuraavaa.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="nexttothisUID">Connectionin UID, jota halutaan etsiä listofconnectionuids listalta ja antaa kyseisestä komponentista seuraavan kohteen UID. Jos -1, niin käytetään luokan omaa indeksilaskuria.</param>
        /// <param name="validconnections">bool, tämä määrittää käytetäänkö kumpaa listaa: listofvalidconnectionuids vai listofconnectionuids.</param>
        /// <param name="reportingtype">Raportointityyppi, määrittää milloin virheet raportoidaan.</param>
        /// <returns>Palauttaa seuraavan Connection instanssin UID:n listofconnectionuids listalta. Jos -3, niin epämääräinen virhe, jos -2 = niin ei yhtään kohdetta listalla ja jos -1 = niin indeksi yli viimeisen elementin.</returns>
        public long IterateThroughReturnNext(string kutsuja, long nexttothisUID=-1, bool validconnections=false, int reportingtype=(int)reportingType.REPORT_ERROR_ALWAYS)
        {
            string functionname="->(IHS)IterateThroughReturnNext";
            if (this.classini.IsClassInitialized==true) {
                SortedList<long, long> connectionslist = validconnections ? this.listofvalidconnectionuids : this.listofconnectionuids;

                if (connectionslist.Count == 0) {
                    return -2; // Ei yhtään kohdetta listalla
                }

                if (nexttothisUID != -1) {
                    int index = connectionslist.IndexOfKey(nexttothisUID);
                    if (index == -1) {
                        if (reportingtype == (int)reportingType.REPORT_ERROR_ALWAYS || reportingtype == (int)reportingType.REPORT_IF_NO_CONNECTION_WITH_SUCH_UID) {
                            this.proghmi.sendError(kutsuja+functionname, "UID not found in the list: " + nexttothisUID, -1264, 4, 4);
                        }
                        return -4; // Epämääräinen virhe
                    }
                    index++;
                    if (index >= connectionslist.Count) {
                        return -1; // Indeksi yli viimeisen elementin
                    }
                    return connectionslist.Keys[index];
                } else {
                    this.validinnerindex++;
                    if (this.validinnerindex >= connectionslist.Count) {
                        return -1; // Indeksi yli viimeisen elementin
                    }
                    return connectionslist.Keys[this.validinnerindex];
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname, "Class not initialized", -1284, 4, 4);
                return -3;
            }
        }       

    }

    /// <summary>
    /// Tämä luokka vastaa BlockAtomValues kohteiden säilyttämisestä
    /// </summary> 
    public class BlockHandles
    {

        /// <summary>
        /// Käyttöliittymän referenssi
        /// </summary> 
        private ProgramHMI proghmi;

        /// <summary> Yksittäisen BlockHandles objektin instanssin oma uniqrefnum eli UID </summary>
        public long OwnUID { get; set; }

        /// <summary>
        /// Tämän BlockHandles vanhemman UID
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Tämän BlockHandles isovanhemman UID
        /// </summary>
        public long GranParentUID { get; set; }

        /// <summary>
        /// Referenssi ObjectIndexeriin, joka pitää ylhäällä, mitä objekteja olemme luoneet ohjelmaan, joiden ID:t täytyy olla rekisteröitynä
        /// </summary>
        private ObjectIndexer objectIndexer;

        /// <summary>
        /// Tämän luokan instanssi pitää kirjaa, onko luokka initialisoitu oikein
        /// </summary> 
        private InitializationCounterClass classini;

        /// <summary>
        /// UID:n perusteella järjestetty lista blockhandle objektin instansseista
        /// </summary>
        private SortedList<long, BlockHandle> listofblockhandle;

        /// <summary>
        /// Sisäinen indeksi, jossa string key arvo on yhdistelmä iterationclass arvoa ja indexnum arvoa tyyliin, mikäli iterationclass on esim. 3 ja indexnum 2, niin tällöin key arvo on 3-2 ja long arvo uid, joka löytyy listofblockhandle listalta
        /// </summary>
        private SortedList<string, long> blockhandleuidindex;

        /// <summary>
        /// Apumuuttuja jolla voidaan iteroida läpi yhden iterationclass listan kohteet. Huomaa, että jos iterationclass muuttuu matkalla, niin tämä muuttuja ei nollaudu. Pelkästään First käsky nollaa tämän muuttujan
        /// </summary> 
        private int lastiterationclassindexnum=0;

        /// <summary>
        /// Alustaa uuden BlockHandles-instanssin, joka vastaa kahvojen tietojen ylläpidosta
        /// </summary>
        /// <param name="kutsuja">string, Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="parentuid">long, Tämän BlockHandlesin vanhemman UID.</param>
        /// <param name="granparentuid">long, Tämän BlockHandlesin isovanhemman UID.</param>
        /// <param name="prohmi">ProgramHMI, Käyttöliittymän referenssi.</param>
        /// <param name="objind">ObjectIndexer, ObjectIndexer-referenssi.</param>
        /// <returns> {void} </returns>
        public BlockHandles(string kutsuja, long parentuid, long granparentuid, ProgramHMI prohmi, ObjectIndexer objind)
        {
            string functionname = "->(BHS)BlockHandles";
            this.proghmi = prohmi;
            this.objectIndexer = objind;
            this.ParentUID = parentuid;
            this.GranParentUID = granparentuid;
            this.classini = new InitializationCounterClass(kutsuja + functionname, 0);
            this.OwnUID = this.objectIndexer.AddObjectToIndexer(kutsuja + functionname, parentuid, (int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_OPERATIONBLOCK_BLOCKHANDLES_400, -1, (int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1, granparentuid);
            this.listofblockhandle = new SortedList<long, BlockHandle>();
            this.blockhandleuidindex = new SortedList<string, long>();
            
            if (this.OwnUID >= 0) {
                int respo = this.objectIndexer.SetObjectToIndexerWithErrorReport(kutsuja + functionname, this.OwnUID, this, (int)ObjectIndexer.rewriteOldObjectReference.ALWAYS_REWRITE_OBJECT_REFERENCE_0);
                if (respo >= 0) {
                    this.classini.AddClassOkayByNumber(kutsuja + functionname, 1);
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Failed to set object to indexer with error report! Response:"+respo, -1295, 4, 4);
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "Failed to add object to indexer! Response:"+this.OwnUID, -1296, 4, 4);
            }
        }

        /// <summary>
        /// Palauttaa BlockHandle objektin UID numeron perusteella
        /// </summary>
        /// <param name="kutsuja">string, Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="UIDtoseek">long, UID arvo, jolla kohdetta etsitään tämän BlockHandles kohteen listoilta</param>
        /// <param name="reportnotfound">bool, jos true, antaa virheviestin, mikäli kohdetta ei löytynyt UID arvolla. Jos false, niin ei raportoi löytymättömästä kohteesta vaan palauttaa vain null.</param>
        /// <returns>{BlockHandle} Palauttaa BlockHandle:n jota on etsitty UIDtoseek arvolla. Palauttaa null, jos ei löydä kohdetta listalta.</returns>
        public BlockHandle ReturnBlockHandleByUID(string kutsuja, long UIDtoseek, bool reportnotfound=true)
        {
            string functionname="->(BHS)ReturnBlockHandleUIDByClassIndex";
            if (this.classini.IsClassInitialized==true) {

                if (this.listofblockhandle.IndexOfKey(UIDtoseek)>-1) {
                    return this.listofblockhandle[UIDtoseek];
                } else {
                    if (reportnotfound==true) {
                        this.proghmi.sendError(kutsuja + functionname, "No such UID in BlockHandle list! UIDtoseek:" + UIDtoseek, -1352, 4, 4);
                    }
                    return null;
                }

            } else {
                this.proghmi.sendError(kutsuja + functionname, "Class not initialized! Response:" + this.classini.ClassOkayNumber, -1351, 4, 4);
                return null;
            }                
        }                    

        /// <summary>
        /// Tämä metodi luo Blokki objektille kahvat ja yhdistää niihin niitä vastaavan connectionrectangle uid:n. Palauttaa itse vastaavan blokin kahvan (BlockHandle) UID:n
        /// </summary>
        /// <param name="kutsuja">string, Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="iterationclass">int enum, Iterationclass arvo, joka on sama kuin ConnectionRectangle.connectionBoxType enumeraation arvo.</param>
        /// <param name="indexnum">int, Indexnum arvo, joka alkaa 0:sta jokaisen iterationclass:in kohdalla erikseen. Eli jos iterationclass==1 (huomaa, että iterationclass 0:aa ei ole), niin indexnum on sen ensimmäiselle objektille 0, sitten 1, 2, 3 jne. </param>
        /// <param name="connectionrectangleuid">ConnectionRectanglen UID arvo. Kyseinen ConnectionRectangle on tiiviisti yhteydessä tämän kyseisen BlockHandle instanssin kanssa.</param>
        /// <param name="blockatomvalueref">BlockAtomValue-referenssi.</param>
        /// <returns>{long} Palauttaa luodun BlockHandle objektin UID:n. Palauttaa miinusmerkkisen arvon, jos tuli virhe objektin luonnissa. </returns>
        public long CreateHandle(string kutsuja, int iterationclass, int indexnum, long connectionrectangleuid, BlockAtomValue blockatomvalueref)
        {
            string functionname = "->(BHS)CreateHandle";

            if (this.classini.IsClassInitialized==true) {
                long newuid = this.objectIndexer.AddObjectToIndexer(kutsuja + functionname, this.OwnUID, (int)ObjectIndexer.indexerObjectTypes.ACTIONCENTRE_OPERATIONBLOCK_BLOCKHANDLE_401, -1, (int)ObjectIndexer.objectIndexerErrorReportTypes.NORMAL_ERROR_REPORTING_1, this.ParentUID);
                
                if (newuid >= 0) {
                    BlockHandle newHandle = new BlockHandle(kutsuja+functionname, newuid, this.OwnUID, this.ParentUID, connectionrectangleuid, iterationclass, indexnum, (int)BlockHandle.externalBlockAtomValue.USE_CONNECTIONRECTANGLE_BLOCKATOMVALUE_1, blockatomvalueref);
                    int respo = this.objectIndexer.SetObjectToIndexerWithErrorReport(kutsuja + functionname, newuid, newHandle, (int)ObjectIndexer.rewriteOldObjectReference.ALWAYS_REWRITE_OBJECT_REFERENCE_0);
                    if (respo >= 0) {
                        this.listofblockhandle.Add(newuid, newHandle);
                        string indexkey = iterationclass + "-" + indexnum;
                        this.blockhandleuidindex.Add(indexkey, newuid);
                        return newuid;
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Failed to set BlockHandle object to indexer with error report", -1298, 4, 4);
                        return -10;
                    }
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Failed to add object to indexer! Response:" + newuid, -1297, 4, 4);
                    return -11;
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "Class not initialized! Response:" + this.classini.ClassOkayNumber, -1310, 4, 4);
                return -12;
            }
        }

        /// <summary>
        /// Etsii iterationclass (joka on connectionBoxType enumeraatio) tiedon perusteella sekä sisäisen järjestyslukuindeksin indexnumtoseek perusteella BlockHandle objektin UID numeroa muodostamalla kahdesta indeksistä yhden string muuttujan ja käyttämällä sitä indeksinä.
        /// </summary>
        /// <param name="kutsuja">string, Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="iterationclasstoseek">int enum, iterationclass (joka on connectionBoxType enumeraatio) tieto, joka määrää pääluokan indeksin haussa erillisindeksistä. </param>
        /// <param name="indexnumtoseek">int, listakohtainen indeksinumero, jolla haetaan kohdetta iterationclass ylätason indeksin sisältä </param>
        /// <param name="alertemptyindex">bool, jos true, antaa virheilmoituksen, mikäli koko indeksilista oli tyhjä (kaikkien iteration class indeksien osalta), muussa tapauksessa false</param>
        /// <returns>{long} Palauttaa -1, jos ei kohdetta kyseisellä indeksillä (ehkä päästy kohdelistaus jo loppuun varsinkin jos edellinen arvo oli jokin kunnollinen). Palauttaa -2, jos ei yhtään kohdetta indeksissä (voi olla virhe, mutta saattaa olla jossain harvinaisessa tapauksessa myös totta.
        /// Jos 0 tai positiivinen luku, niin palauttaa BlockHandle objektin UID:n. Jos palauttaa arvon &lt; 2, niin tällöin kyseessä on aina virhe.
        /// </returns>
        public long ReturnBlockHandleUIDByClassIndex(string kutsuja, int iterationclasstoseek, int indexnumtoseek, bool alertemptyindex=true)
        {
            string functionname="->(BHS)ReturnBlockHandleUIDByClassIndex";
            long retVal=-10;
            if (this.classini.IsClassInitialized==true) {
                string seekkey=iterationclasstoseek+"-"+indexnumtoseek;                
                if (this.blockhandleuidindex.Count>0) {
                    if (iterationclasstoseek>=(int)ConnectionRectangles.connectionBoxType.MIN_VALUE_INDEX && iterationclasstoseek<=(int)ConnectionRectangles.connectionBoxType.MAX_VALUE_INDEX) {
                        if (this.blockhandleuidindex.IndexOfKey(seekkey)>-1) {
                            retVal=this.blockhandleuidindex[seekkey];
                        } else {
                            retVal=-1; // Ei kyseistä indeksinumeroa - ehkä saavutettiin jo loppu
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Invalid iteration class! Iterationclass:" + iterationclasstoseek, -1348, 4, 4);
                        retVal=-13;
                    }
                } else {
                    if (alertemptyindex==true) {
                        this.proghmi.sendError(kutsuja + functionname, "No entry in index! Seekkey:" + seekkey, -1347, 4, 4);
                    }
                    retVal=-2;
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "Class not initialized! Response:" + this.classini.ClassOkayNumber, -1346, 4, 4);
                retVal=-12;
            }                
            return retVal;
        }

        /// <summary>
        /// Etsii iterationclass (joka on connectionBoxType enumeraatio) tiedon perusteella sekä sisäisen järjestyslukuindeksin indexnumtoseek perusteella BlockHandle objektin UID numeroa ja palauttaa iterationclass indeksin alta ensimmäisen kohteen sekä nollaa laskurin
        /// </summary>
        /// <param name="kutsuja">string, Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="iterationclasstoseek">int enum, iterationclass (joka on connectionBoxType enumeraatio) tieto, joka määrää pääluokan indeksin haussa erillisindeksistä. </param>
        /// <returns>{long} Palauttaa -1, jos ei kohdetta kyseisellä indeksillä (ehkä päästy kohdelistaus jo loppuun varsinkin jos edellinen arvo oli jokin kunnollinen). Palauttaa -2, jos ei yhtään kohdetta indeksissä (voi olla virhe, mutta saattaa olla jossain harvinaisessa tapauksessa myös totta).
        /// Jos 0 tai positiivinen luku, niin palauttaa BlockHandle objektin UID:n. Jos palauttaa arvon &lt; 2, niin tällöin kyseessä on aina virhe.
        /// </returns>
        public long ReturnBlockHandleUIDFirst(string kutsuja, int iterationclasstoseek)
        {
            string functionname="->(BHS)ReturnBlockHandleUIDFirst";
            long retVal=-20;
            this.lastiterationclassindexnum=0;
            if (this.classini.IsClassInitialized==true) {
                retVal=this.ReturnBlockHandleUIDByClassIndex(kutsuja+functionname,iterationclasstoseek,this.lastiterationclassindexnum,false);                
            } else {
                this.proghmi.sendError(kutsuja + functionname, "Class not initialized! Response:" + this.classini.ClassOkayNumber, -1349, 4, 4);
                retVal=-22;
            }                
            return retVal;            
        }

        /// <summary>
        /// Etsii iterationclass (joka on connectionBoxType enumeraatio) tiedon perusteella sekä sisäisen järjestyslukuindeksin indexnumtoseek perusteella BlockHandle objektin UID numeroa ja palauttaa iterationclass indeksin alta järjestyksessä seuraavan kohteen
        /// </summary>
        /// <param name="kutsuja">string, Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="iterationclasstoseek">int enum, iterationclass (joka on connectionBoxType enumeraatio) tieto, joka määrää pääluokan indeksin haussa erillisindeksistä. </param>
        /// <returns>{long} Palauttaa -1, jos ei kohdetta kyseisellä indeksillä (ehkä päästy kohdelistaus jo loppuun varsinkin jos edellinen arvo oli jokin kunnollinen). Palauttaa -2, jos ei yhtään kohdetta indeksissä (voi olla virhe, mutta saattaa olla jossain harvinaisessa tapauksessa myös totta).
        /// Jos 0 tai positiivinen luku, niin palauttaa BlockHandle objektin UID:n. Jos palauttaa arvon &lt; 2, niin tällöin kyseessä on aina virhe.
        /// </returns>
        public long ReturnBlockHandleUIDNext(string kutsuja, int iterationclasstoseek)
        {
            string functionname="->(BHS)ReturnBlockHandleUIDFirst";
            long retVal=-30;
            if (this.classini.IsClassInitialized==true) {
                this.lastiterationclassindexnum++;
                retVal=this.ReturnBlockHandleUIDByClassIndex(kutsuja+functionname,iterationclasstoseek,this.lastiterationclassindexnum,true);                
            } else {
                this.proghmi.sendError(kutsuja + functionname, "Class not initialized! Response:" + this.classini.ClassOkayNumber, -1350, 4, 4);
                retVal=-32;
            }                
            return retVal;            
        }        

    }

    /// <summary>
    /// Tämän luokan instanssi säilöö yhden Blokin kahvan tiedot, johon kuuluu yksi sisäinen tai ulkoinen BlockAtomValue
    /// </summary> 
    public class BlockHandle
    {
        /// <summary>
        /// int enum, ConnectionRectangle.connectionBoxType:n enumeraatio, joita taitaa olla tällähetkellä vaihtoehdot 1 - 3
        /// </summary>
        public int IterationClass { get; set; }
        /// <summary>
        /// Indeksinumero, joka on annettu järjestyksessä ConnectionRectanglen luontia varten ja nyt annetaan tälle objektille tiedoksi
        /// </summary>
        public int IndexNum { get; set; }

        /// <summary>
        /// ConnectionRectangle objektin UID (tai jonkin muun objektin UID), johon tämä objekti yhdistyy suoralla linkillä (molemmat tietävät toistensa UID:n)
        /// </summary>
        public long AssosiatedObjectUID { get; set; }

        /// <summary> Yksittäisen BlockHandle objektin instanssin oma uniqrefnum eli UID </summary>
        public long OwnUID { get; set; }

        /// <summary>
        /// Tämän BlockHandle vanhemman UID
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Tämän BlockHandle isovanhemman UID
        /// </summary>
        public long GranParentUID { get; set; }

        private BlockAtomValue handleblockatomvalue;
        /// <summary>
        /// Palauttaa kahvan käytettävän BlockAtomValue:n
        /// </summary>
        public BlockAtomValue ReturnBlockAtomValueRef {
            get { return this.handleblockatomvalue; }
        }

        /// <summary>
        /// Tämä enumeraatio kertoo, käytetäänkö sisäistä BlockAtomValue tietoa kyseisen blokin kahvoissa, vai itseasiassa ulkoisia objekteja, kuten ConnectionRectangle:n BlockAtomValueta jne.
        /// </summary>
        public enum externalBlockAtomValue {
            /// <summary>
            /// Käytetään aina vain sisäistä erikseen luotua BlockAtomValue objektia
            /// </summary> 
            NEVER_USE_EXTERNAL_BLOCKATOMVALUE_0=0,
            /// <summary>
            /// Käytetään ulkoista ConnectionRectanglen BlockAtomValue objektia tiedon säilyttämiseen
            /// </summary> 
            USE_CONNECTIONRECTANGLE_BLOCKATOMVALUE_1=1
        };

        /// <summary>
        ///  Tähän muuttujaan on merkattu, onko käytetty ulkopuolista BlockAtomValue:ta vai erillistä sisäistä. Käyttää externalBlockAtomValue enumeraatiota.
        /// </summary>
        public int ExternalBlockAtomValueInfo { get; set; }


        /// <summary>
        /// Alustaa uuden BlockHandle-instanssin.
        /// </summary>
        /// <param name="kutsuja">string, Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="ownuid">long, Tämän objektin oma UID, jonka äitiobjektin instanssi on jo luonut tätä varten.</param>
        /// <param name="parentuid">Tämän BlockHandlen vanhemman UID.</param>
        /// <param name="granparentuid">Tämän BlockHandlen isovanhemman UID.</param>
        /// <param name="assobjuid">ConnectionRectangle UID (tai jonkin muun objektin UID), johon tämä objekti yhdistyy suoralla linkillä.</param>
        /// <param name="iterclass">IterationClass arvo, joka vastaa ConnectionRectangle.connectionBoxTypes enumeraatiota</param>
        /// <param name="indexnume">IndexNum arvo.</param>
        /// <param name="useconnectionrectangleblockatomvalue">int enum, käyttää BlockHandle.externalBlockAtomValue enumeraatiota siinä, käyttääkö omaa vai ulkopuolista BlockAtomValue instanssia.</param>
        /// <param name="extBlockAtomValue">BlockAtomValue, ulkopuolinen BlockAtomValue objektin referenssi (kuten esim. ConnectionRectanglen BlockAtomValue objektin referenssi), jota käytetään oman BlockAtomValuen sijasta.</param>
        /// <returns> {void} </returns> 
        public BlockHandle(string kutsuja, long ownuid, long parentuid, long granparentuid, long assobjuid, int iterclass, int indexnume, int useconnectionrectangleblockatomvalue=(int)externalBlockAtomValue.USE_CONNECTIONRECTANGLE_BLOCKATOMVALUE_1, BlockAtomValue extBlockAtomValue=null)
        {
            string functionname="->(BH)BlockHandle";
            this.OwnUID=ownuid;
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.AssosiatedObjectUID=assobjuid;
            this.IterationClass=iterclass;
            this.IndexNum=indexnume;

            this.ExternalBlockAtomValueInfo=useconnectionrectangleblockatomvalue;
            if (useconnectionrectangleblockatomvalue==(int)externalBlockAtomValue.NEVER_USE_EXTERNAL_BLOCKATOMVALUE_0) {
                this.handleblockatomvalue=new BlockAtomValue(); // Luodaan oma BlockAtomValue
            } else {
                this.handleblockatomvalue=extBlockAtomValue; // Käytetään ulkopuolisen objektin BlockAtomValue kohdetta
            }
        }    
    }

