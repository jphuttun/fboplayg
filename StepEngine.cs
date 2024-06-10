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
    /// Tämä luokka pitää sisällään tiedot, jossa voidaan säilyttää koko blokkirakenteen käyttäjän haluamien sääntöjen ajamiseksi
    /// </summary>
    public class StepEngine
    {

        /// <summary>
        /// Järjestetty lista UID:n mukaan, joka säilyttää kokoelmaa StepEngineSuperBlokkeja
        /// </summary>
        public SortedList<long, StepEngineSuperBlock> listofStepEngineSuperBlocks;

        /// <summary>
        /// ActionCentreUI luokan referenssi, jota kautta voidaan ajaa blokkirakennetta
        /// </summary>
        private ActionCentreUI acUI;

        /// <summary>
        /// Käyttöliittymäluokan referenssi
        /// </summary>
        private ProgramHMI proghmi; 

        /// <summary>
        /// Referenssi ObjectIndexer luokkaan, joka jakaa uniqrefnumit (eli UID) tiedot jokaiselle ohjelmaan luotavalle pysyvälle objektille
        /// </summary>
        private ObjectIndexer objindexer;

        /// <summary>
        /// Tämän objektin vanhemman UID tieto
        /// </summary>
        public long ParentUID { get; set; } 

        /// <summary>
        /// Tämän objektin oma UID tieto
        /// </summary>
        public long OwnUID { get; set; }

        public StepEngine(string kutsuja, long parentuid, ProgramHMI prohmi, ObjectIndexer objindxr, ActionCentreUI actcenui)
        {
            string functionname="->(SE)StepEngine";
            this.proghmi=prohmi;
            this.objindexer=objindxr;
            this.acUI=actcenui;
            this.listofStepEngineSuperBlocks=new SortedList<long, StepEngineSuperBlock>();
            this.ParentUID=parentuid;
            this.OwnUID = this.objindexer.AddObjectToIndexer(kutsuja+functionname, this.ParentUID, (int)ObjectIndexer.indexerObjectTypes.STEP_ENGINE_OBJECT_2000,-1);
            if (this.OwnUID<0) {
                this.proghmi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+this.OwnUID+" ParentUid:"+this.ParentUID,-1071,4,4);
            }            
        }

        /// <summary>
        /// Lisää uuden StepEngineSuperBlock-elementin ja rekisteröi sen ObjectIndexer-luokkaan.
        /// Adds a new StepEngineSuperBlock element and registers it with the ObjectIndexer class.
        /// </summary>
        /// <param name="caller">string, Caller's path.</param>
        /// <returns> {long}, The UID of the newly added StepEngineSuperBlock. Returns -2, if error! </returns>
        public long AddElement(string caller)
        {
            string functionname="->(SE)AddElement";
            long uid = this.objindexer.AddObjectToIndexer(caller+functionname, this.OwnUID, (int)ObjectIndexer.indexerObjectTypes.STEP_ENGINE_SUPER_BLOCK_2020,-1);
            if (uid>=0) {
                if (this.listofStepEngineSuperBlocks.IndexOfKey(uid)==-1) {
                    this.listofStepEngineSuperBlocks.Add(uid, new StepEngineSuperBlock(caller+functionname, this.OwnUID, uid, this.proghmi, this.objindexer, this.acUI));
                } else {
                    this.proghmi.sendError(caller+functionname,"UID was already in list! UID:"+uid,-1026,4,4); // Tätä ei pitäisi koskaan päästä tapahtumaan
                    uid=-2;
                }
            } else {
                this.proghmi.sendError(caller+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+uid+" ParentUid:"+this.OwnUID,-1072,4,4);
                uid=-3;
            }            
            return uid;
        }

        /// <summary>
        /// Adds an existing StepEngineInstruction to a new StepEngineSuperBlock.
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="instruction">StepEngineInstruction, the instruction to add.</param>
        /// <returns>{KeyValuePair long, long}, where key is the UID of the new StepEngineSuperBlock and value is the UID of the new StepEngineInstruction.
        /// Jos palauttaa KeyValuePair:ssa jomman kumman elementeistä miinusmerkkisenä, niin tällöin kyseisen elementin kanssa on tullut virhe!</returns>
        public KeyValuePair<long, long> AddInstructionToNewSuperBlock(string caller, StepEngineInstruction instruction)
        {
            string functionname = "->(SE)AddInstructionToNewSuperBlock";
            // Create new StepEngineSuperBlock
            long superBlockUID = this.objindexer.AddObjectToIndexer(caller + functionname, this.OwnUID, (int)ObjectIndexer.indexerObjectTypes.STEP_ENGINE_SUPER_BLOCK_2020, -1);
            if (superBlockUID>=0) {
                var newSuperBlock = new StepEngineSuperBlock(caller + functionname, this.OwnUID, superBlockUID, this.proghmi, this.objindexer, this.acUI);
                this.listofStepEngineSuperBlocks.Add(superBlockUID, newSuperBlock);

                // Add StepEngineInstruction to the new StepEngineSuperBlock
                long instructionUID = newSuperBlock.AddInstruction(caller + functionname, instruction);
                return new KeyValuePair<long, long>(superBlockUID, instructionUID);                
            } else {
                this.proghmi.sendError(caller+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+superBlockUID+" ParentUid:"+this.OwnUID,-1073,4,4);
                return new KeyValuePair<long, long>(-1,-1);
            }
        }

        /// <summary>
        /// Tries to add an existing StepEngineInstruction to an existing StepEngineSuperBlock.
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="superBlockUID">long, UID of the StepEngineSuperBlock.</param>
        /// <param name="instructionUID">long, UID of the StepEngineInstruction.</param>
        /// <param name="instruction">StepEngineInstruction, the instruction to add.</param>
        /// <param name="createIfNotFound">bool, indicates whether to create new elements if not found.</param>
        /// <returns>KeyValuePair<long, long>, where key is the UID of the StepEngineSuperBlock and value is the UID of the StepEngineInstruction. If either is not found, -1 is returned for that element.</returns>
        public KeyValuePair<long, long> AddInstructionToExistingSuperBlock(string caller, long superBlockUID, long instructionUID, StepEngineInstruction instruction, bool createIfNotFound)
        {
            string functionname = "->(SE)AddInstructionToExistingSuperBlock";

            // Check if the super block exists
            if (listofStepEngineSuperBlocks.IndexOfKey(superBlockUID)>-1) {
                // Add instruction to existing super block
                long retinstructionUID = listofStepEngineSuperBlocks[superBlockUID].TryAddInstructionToUID(caller+functionname, instructionUID, instruction);
                return new KeyValuePair<long, long>(superBlockUID, retinstructionUID);
            } else {
                // Create a new super block and add instruction
                return this.AddInstructionToNewSuperBlock(caller+functionname, instruction);
            }
        }

        /// <summary>
        /// Creates a new StepEngineInstruction instance and returns its reference.
        /// </summary>
        /// <param name="caller">string, Caller's path.</param>
        /// <returns>{StepEngineInstruction}, A reference to the newly created StepEngineInstruction instance.</returns>
        public StepEngineInstruction CreateNewInstruction(string caller)
        {
            string functionname = "->(SE)CreateNewInstruction";
            StepEngineInstruction newInstruction;
            long superblockuid=this.AddElement(caller+functionname);

            if (superblockuid>=0) {
                // Create a new StepEngineInstruction instance
                long instructionuid=this.listofStepEngineSuperBlocks[superblockuid].AddElement(caller+functionname);
                if (instructionuid>=0) {
                    if (this.listofStepEngineSuperBlocks[superblockuid].listofStepEngineInstructions.IndexOfKey(instructionuid)>-1) {
                        newInstruction=this.listofStepEngineSuperBlocks[superblockuid].listofStepEngineInstructions[instructionuid];
                    } else {
                        this.proghmi.sendError(caller+functionname,"Couldn't find stepengineinstruction! SuperBlockUID:"+superblockuid+" UID:"+instructionuid,-1031,4,4); // Tätä ei pitäisi koskaan päästä tapahtumaan
                        newInstruction=null;                        
                    }
                } else {
                    this.proghmi.sendError(caller+functionname,"Couldn't create stepengineinstruction! SuperBlockUID:"+superblockuid+" UID:"+instructionuid,-1030,4,4); // Tätä ei pitäisi koskaan päästä tapahtumaan
                    newInstruction=null;                    
                }
            } else {
                this.proghmi.sendError(caller+functionname,"Couldn't create superblock! It already existed! UID:"+superblockuid,-1027,2,4); // Tätä ei pitäisi koskaan päästä tapahtumaan
                newInstruction=null;
            }

            return newInstruction;
        }

        /// <summary>
        /// Finds a StepEngineInstruction based on the provided StepEngineSuperBlock and StepEngineInstruction UIDs.
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="superBlockUID">long, UID of the StepEngineSuperBlock.</param>
        /// <param name="instructionUID">long, UID of the StepEngineInstruction.</param>
        /// <returns>{StepEngineInstruction} the found instruction or null if error.</returns>
        public StepEngineInstruction FindInstructionByUID(string caller, long superBlockUID, long instructionUID)
        {
            string functionname = "->(SE)FindInstructionByUID";

            if (listofStepEngineSuperBlocks.IndexOfKey(superBlockUID)>-1) {
                if (listofStepEngineSuperBlocks[superBlockUID].listofStepEngineInstructions.IndexOfKey(instructionUID)>-1) {
                    return listofStepEngineSuperBlocks[superBlockUID].listofStepEngineInstructions[instructionUID];
                } else {
                    this.proghmi.sendError(caller + functionname, "Instruction with UID " + instructionUID + " not found in SuperBlock " + superBlockUID, -1032, 4, 4);
                    return null;
                }
            } else {
                this.proghmi.sendError(caller + functionname, "SuperBlock with UID " + superBlockUID + " not found.", -1033, 4, 4);
                return null;
            }
        }             


        /// <summary>
        /// Removes a StepEngineSuperBlock element by its UID.
        /// </summary>
        /// <param name="caller">Caller's path.</param>
        /// <param name="uid">UID of the StepEngineSuperBlock to remove.</param>
        /// <returns>{bool}, true if successfully removed, false otherwise.</returns>
        public bool RemoveElement(string caller, long uid)
        {
            string functionname = "->(SE)RemoveElement";
            if (this.listofStepEngineSuperBlocks.ContainsKey(uid)) {
                // First, remove all StepEngineInstructions within the super block
                this.listofStepEngineSuperBlocks[uid].RemoveAllElements(caller + functionname);

                // Now, remove the super block itself
                this.listofStepEngineSuperBlocks.Remove(uid);
                return true;
            } else {
                // Log an error if the super block does not exist
                // Error handling code goes here
                this.proghmi.sendError(caller+functionname,"No such removing UID! UID:"+uid,-1024,4,4);
                return false;
            }
        }

        /// <summary>
        /// Removes all StepEngineSuperBlock elements and its sub elements within all super blocks.
        /// </summary>
        /// <param name="caller">string, Caller's path.</param>
        /// <returns>{void}</returns>
        public void RemoveAllElements(string caller)
        {
            string functionname = "->(SE)RemoveAllElements";
            if (this.listofStepEngineSuperBlocks.Count>0) {

                // Iterate over each StepEngineSuperBlock and remove its elements
                foreach (var superBlock in this.listofStepEngineSuperBlocks.Values) {
                    superBlock.RemoveAllElements(caller + functionname);
                }

                // Clear the list after all super blocks have been processed
                this.listofStepEngineSuperBlocks.Clear();

                // Additional cleanup or logging can be done here

            }
        }      

    }

    /// <summary>
    /// Tämä luokka pitää sisällään tiedot, joissa useasta StepEngineInstruction objektista kasataan suurempi superblokki, jota voidaan ajaa sellaisenaan
    /// </summary>
    public class StepEngineSuperBlock
    {
        /// <summary>
        /// Järjestetty lista UID:n mukaan, joka säilyttää kokoelmaa StepEngineInstruction objekteja
        /// </summary>
        public SortedList<long, StepEngineInstruction> listofStepEngineInstructions;

        /// <summary>
        /// ActionCentreUI luokan referenssi, jota kautta voidaan ajaa blokkirakennetta
        /// </summary>
        private ActionCentreUI acUI;

        /// <summary>
        /// Referenssi ObjectIndexer luokkaan, joka jakaa uniqrefnumit (eli UID) tiedot jokaiselle ohjelmaan luotavalle pysyvälle objektille
        /// </summary>
        private ObjectIndexer objindexer; 

        /// <summary>
        /// Käyttöliittymäluokan referenssi
        /// </summary>
        private ProgramHMI proghmi; 

        /// <summary>
        /// Tämän objektin vanhemman UID tieto
        /// </summary>
        private long ParentUID { get; set; } 

        /// <summary>
        /// Tämän objektin oma UID tieto
        /// </summary>
        private long OwnUID { get; set; }                               

        public StepEngineSuperBlock(string kutsuja, long parentuid, long objuid, ProgramHMI prohmi, ObjectIndexer objindexr, ActionCentreUI actcentui)
        {
            string functionname="->(SESB)StepEngineSuperBlock";
            this.proghmi=prohmi;
            this.objindexer=objindexr;
            this.acUI=actcentui;            
            this.listofStepEngineInstructions=new SortedList<long, StepEngineInstruction>();
            this.ParentUID=parentuid;
            this.OwnUID=objuid;
        }

        /// <summary>
        /// Lisää uuden StepEngineInstruction-elementin ja rekisteröi sen ObjectIndexer-luokkaan.
        /// Adds a new StepEngineInstruction element and registers it with the ObjectIndexer class.
        /// </summary>
        /// <param name="caller">Caller's path.</param>
        /// <returns> {long}, The UID of the newly added StepEngineInstruction.</returns>
        public long AddElement(string caller)
        {
            string functionname = "->(SESB)AddElement";

            return this.AddInstruction(caller+functionname, null, true, false);
        }

        /// <summary>
        /// Adds an existing StepEngineInstruction to this StepEngineSuperBlock.
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="instruction">StepEngineInstruction, the instruction to add.</param>
        /// <param name="createnewelement">bool, flag to create a new element if instruction is null.</param>
        /// <param name="overwriteold"> bool, flag to overwrite old instruction, if it is there already </param>
        /// <returns>{long}, The UID of the newly added StepEngineInstruction. Returns smaller than 0, if error! </returns>
        public long AddInstruction(string caller, StepEngineInstruction instruction, bool createnewelement=false, bool overwriteold=false)
        {
            string functionname = "->(SESB)AddInstruction";
            long retinstuid=-1;
            // Generate a UID for the new instruction and add it to the list
            long instructionUID = this.objindexer.AddObjectToIndexer(caller + functionname, this.OwnUID, (int)ObjectIndexer.indexerObjectTypes.STEP_ENGINE_INSTRUCTION_2040, -1);
            if (instructionUID>=0) {
                if (createnewelement==true) {
                    instruction=new StepEngineInstruction(caller + functionname, this.OwnUID, instructionUID, this.proghmi, this.objindexer, this.acUI);
                }

                if (instruction==null) {
                    this.proghmi.sendError(caller+functionname,"Instruction was null! UID:"+instructionUID,-1028,4,4); 
                    retinstuid=-2;
                } else {
                    if (this.listofStepEngineInstructions.ContainsKey(instructionUID)) {
                        // UID already exists, overwrite the existing instruction
                        if (overwriteold==true) {
                            this.listofStepEngineInstructions[instructionUID] = instruction;
                            retinstuid=instructionUID;
                        } else {
                            this.proghmi.sendError(caller+functionname,"Couldn't overwrite instruction! UID:"+instructionUID,-1029,4,4); // Tätä ei pitäisi koskaan päästä tapahtumaan
                            retinstuid=-3;
                        }
                    } else {
                        this.listofStepEngineInstructions.Add(instructionUID, instruction);
                        retinstuid=instructionUID;
                    }
                }
            } else {
                this.proghmi.sendError(caller+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+instructionUID+" ParentUid:"+this.OwnUID,-1074,4,4);
                retinstuid=-4;
            }
            
            return retinstuid;
        }

        /// <summary>
        /// Tries to add a StepEngineInstruction to a specific UID in the list.
        /// If the UID already exists, it overwrites the existing instruction and instructionUID parameter value.
        /// If the UID does not exist, it adds the instruction as new and returns its UID.
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="instructionUID">long, the UID where the instruction should be added or updated. IF instruction IS ADDED, RETURN value IS DIFFERENT THAN instructionUID!!! </param>
        /// <param name="instruction">StepEngineInstruction, the instruction to add.</param>
        /// <returns>{long}, The UID of the newly added instruction, or same value than instructionUID what was given if overwritten. BE AWARE that if new instance is created, given instructionUID is not same than returned UID! </returns>
        public long TryAddInstructionToUID(string caller, long instructionUID, StepEngineInstruction instruction)
        {
            string functionname = "->(SESB)TryAddInstructionToUID";

            // Register the new instruction UID in ObjectIndexer
            instructionUID=this.AddInstruction(caller+functionname,instruction,false,true);
            return instructionUID;   
        }

        /// <summary>
        /// Removes a StepEngineInstruction element by its UID.
        /// </summary>
        /// <param name="caller">string, Caller's path.</param>
        /// <param name="uid">long, UID of the StepEngineInstruction to remove.</param>
        /// <returns>{bool}, true if successfully removed, false otherwise.</returns>
        public bool RemoveElement(string caller, long uid)
        {
            string functionname = "->(SESB)RemoveElement";
            if (this.listofStepEngineInstructions.ContainsKey(uid)) {
                this.listofStepEngineInstructions.Remove(uid);
                return true;
            } else {
                // Log an error if the instruction does not exist
                // Error handling code goes here
                this.proghmi.sendError(caller+functionname,"No such removing UID! UID:"+uid,-1025,4,4);
                return false;
            }
        }

        /// <summary>
        /// Removes all StepEngineInstruction elements within this super block.
        /// </summary>
        /// <param name="caller">string, Caller's path.</param>
        /// <returns>{void}</returns>
        public void RemoveAllElements(string caller)
        {
            string functionname = "->(SESB)RemoveAllElements";
            this.listofStepEngineInstructions.Clear();
            // Additional cleanup or logging can be done here
        }                     

    }

    /// <summary>
    /// Tämä luokka pitää sisällään periaatteellisen rakenteen, joka tyhjätään joka kutsukertaa varten ja oliot tuhotaan tarvittaessa. Mutta jos rakenne pysyy aina samana, oliot tyhjätään vain tällöin.
    /// Olennaista on pyrkiä ajamaan kohdetta niin pitkään, että sen lopputulos on selvä ja tieto voidaan tallentaa altroute tietona. Tämä tarkoittaa sitä, että konstruktio on edennyt lopetusehtoon tai lopettanut suorittamisen ehtoon, joka odottaa vastausta ulkopuolelta
    /// </summary>
    public class BlockPrincipleStructure
    {
        /// <summary>
        /// Apulista, johon kerätään ne UID arvot, joista blokeista käsin aloitetaan kyseisen blokin ajaminen. Long on kohteen UID ja int on kohteen tyyppi, eli blockTypeEnum
        /// </summary>
        public SortedList<long, int> blocksstartingkeylist;

        /// <summary>
        /// Apulista, johon on kerätty kaikki blokkien UID arvot (eli tällä hetkellä käytännössä sama lista kuin key lista MotherConnectionRectangle lista). Long on kohteen UID ja int on kohteen tyyppi, eli blockTypeEnum
        /// </summary>
        public SortedList<long, int> allblockskeylist;

        /// <summary>
        /// Lista, johon on kerätty kaikki luodut blokkien ActionCentrellä luodut objektit. Int tässä on kohteen altroutevalue.
        /// </summary>
        public SortedList<int, object> constructionblockobjects;

        public BlockPrincipleStructure(string kutsuja)
        {
            string functionname="->(BPS)Constructor";

            this.blocksstartingkeylist = new SortedList<long, int>();
            this.allblockskeylist = new SortedList<long, int>();
            this.constructionblockobjects = new SortedList<int, object>();
        }
    }

    /// <summary>
    /// Tämä luokka hoitaa yhden stepengine blokin tai blokkijärjestelmän tietojen säilytyksestä
    /// </summary>
    public class StepEngineInstruction
    {
        /// <summary>
        /// Käyttöliittymäluokan referenssi
        /// </summary>
        private ProgramHMI proghmi;

        /// <summary>
        /// ActionCentreUI luokan referenssi, jota kautta voidaan ajaa blokkirakennetta
        /// </summary>
        private ActionCentreUI acUI;

        /// <summary>
        /// Referenssi ObjectIndexer luokkaan, joka jakaa uniqrefnumit (eli UID) tiedot jokaiselle ohjelmaan luotavalle pysyvälle objektille
        /// </summary>
        private ObjectIndexer objindexer;

        /// <summary>
        /// Tämän objektin vanhemman UID tieto
        /// </summary>
        public long ParentUID { get; set; } 

        /// <summary>
        /// Tämän objektin oma UID tieto
        /// </summary>
        public long OwnUID { get; set; } 

        /// <summary>
        /// Tämä instanssi pitää sisällään blocksstartingkeylist, allblockskeylist:in sekä constructionobject listin
        /// </summary>
        public BlockPrincipleStructure blockprinc;               

        /// <summary>
        /// Constructor StepEngineInstruction luokalle, joka hoitaa yhden stepengine blokin tai blokkijärjestelmän tietojen säilytyksestä
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="parentuid"> long, tämän objektin vanhemman UID tieto </param>
        /// <param name="objuid"> long, tämän objetin oma UID tieto </param>
        /// <param name="prohmi"> ProgramHMI, Käyttöliittymän referenssi </param>
        /// <param name="objinde"> ObjectIndexer, joka jakaa uniqrefnumit (eli UID) tiedot jokaiselle ohjelmaan luotavalle pysyvälle objektille </param>
        /// <param name="actcenui"> ActionCentreUI, luokan referenssi, jota kautta voidaan ajaa blokkirakennetta </param>
        public StepEngineInstruction(string kutsuja, long parentuid, long objuid, ProgramHMI prohmi, ObjectIndexer objinde, ActionCentreUI actcenui)
        {
            string functionname="->(SEI)StepEngineInstruction";
            this.proghmi=prohmi;
            this.objindexer=objinde;
            this.acUI=actcenui;
            this.ParentUID=parentuid;
            this.OwnUID=objuid;
            this.blockprinc = new BlockPrincipleStructure(kutsuja+functionname);
        }

        /// <summary>
        /// Runs the instruction based on the UID provided in the allblockskeylist. Nähdäkseni ajaa vain yhden kohteen (blokin) Instructionin läpi kerrallaan
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="altroutevalue">int, kyseisen slotin altroutevalue, joka kohteella on kullakin hetkellä </param>
        /// <param name="blockUID">long, UID of the block to be executed.</param>
        /// <param name="oneslot">OneSlot, sen yksittäisen slotin referenssi, jonka kautta haetaan tämän toiminnon tarvitsemat alkuarvot yms. OneSlot objektin instanssin kautta on ObjectIndexeriä hyödyntämällä mahdollista päästä käsiksi myös SlotList luokan instanssiin, johon tämä oneslot kuuluu sekä sitä kautta SmartBot ja PrimaryCode luokkien instansseihin. Tällä tavoin on mahdollista hakea lähtötietoja laajemmastakin spektrumista, kuin pelkästään tähän slottiin liittyen </param>
        /// <returns>{int}, kertoo sen altroutevaluen, jonne saakka RunInstruction käsky pääsi yhden yksittäisen käskyn ajossa. Jos pienempi kuin 0, niin virhe. -100=määrittelemätön virhe, -101=UID:lla ei löytynyt mitään </returns>
        public int RunInstruction(string caller, int altroutevalue, long blockUID, OneSlot oneslot)
        {
            string functionname = "->(SEI)RunInstruction";
            int retVal=-100;

            // Check if the blockUID exists in the allblockskeylist
            if (this.blockprinc.allblockskeylist.TryGetValue(blockUID, out int blockType)) {
                // Run the block using the ActionCentreUI instance
                if (this.acUI.ReturnBPActionsReference!=null) {
                    //return this.acUI.ReturnBPActionsReference.actioncentre.RunBlock(caller + functionname, blockType);

                    // Toteutetaan seuraavasti:
                    // 1.) Otetaan yksi altroute tieto (parametri int altroutevalue) sekä blokkia vastaava (parametri long blockUID)
                    // 2.) Etsitään blokki blockUID tiedolla ActionCentreConstructionHandler olion listofMotherBoxes kokoelmasta ja tarkastetaan, että sen altroutevalue vastaa altroutevalue parametria
                    MotherConnectionRectangle motherrectangleinuse=this.acUI.ReturnAcUIConstructionHandler.ReturnMainBlock(caller+functionname,blockUID,true,true,altroutevalue);

                    // 3.) Otetaan allblockskeylist kokoelmasta talteen blokin tyyppi, jonkalainen ajettava blokki on
                    // -- Tämä on tehty tämän funktion ensimmäisessä if lauseessa, jossa on out int blockType

                    // 4.) Luodaan ajettava blokki ActionCentre luokan CreateBlock käskyllä tai käytetään aiempaa blokkia, joka on tyhjätty uusintakäyttöä varten
                    // -On ehkä järkevintä, että jokaiselle MotherRectangle:lle luodaan sen blokkityyppiä vastaava toimintablokki myös - TEHTY
                    // -Suunnittele asia siten, että toimintablokit saadaan helposti myös tyhjättyä tiedoista sekä tuhottua, jos ne ovat kaukana toiminta-alueesta. 
                    // -Ennenkuin blokkia käytetään ja ennenkuin sen sisään asetetaan arvoja, tarkistetaan onko blokin objekti luotu, vai joudutaanko se luomaan CreateBlock käskyllä - TEHTY (lähes kokonaan)
                    
                    if (motherrectangleinuse!=null) {

                    } else {
                        this.acUI.ReturnRegisteredActionCentreRef.CreateBlock(); // Ei ehkä luoda itse blokkia vaan, oletetaan että se on luotu jo MotherRectanglea luotaessa, joten nyt vain tarkistetaan, onko se luotu ja annetaan virheilmoitus kuten alla
                        this.proghmi.sendError(caller + functionname, "MortherRectangle reference was null! Altrouteval: "+altroutevalue+" Block with UID: " + blockUID, -1199, 4, 4); 
                    }

                    // Pitäskö tehdä (ActionCentre) blokin sisään metodi, jolle annetaan smallBoxRectangles ja blokki käy läpi kaikki UID:it ja päivittää blokille kahvojen arvot.
                    motherrectangleinuse.smallBoxRectangles.IterateThroughConnectionsFirst(caller+functionname,)

                    // 5.) Mennään blokkiin sisälle ja tarkistetaan, onko sen kaikki tulopuolen kahvat saaneet jo arvon (tarvitseeko tämä alussa selvityksen mitkä kaikki kahvat ovat ylipäätään saamassa arvoja)
                    // 5a) Jos tulopuolen kahvat ovat saaneet jo arvonsa, suoritetaan blokki ja lähetetään tulostieto niihin blokkeihin, joihin kyseinen blokki on yhdistetty ja tallennetaan tulostiedot kyseisen blokin kahvoihin. Merkataan tällöin blokki suoritetuksi, eli enää siihen ei tulisi tarvetta palata, ellei sitä tyhjännetä seuraavan tarkastelukierroksen tieltä
                    // 5b) Jos kaikki tulopuolen kahvat eivät ole saaneet vielä arvoaan, ei suoriteta blokkia vaan mennään suorittamaan seuraavaa blokkia.
                    // 6.) Lopetetaan blokkien suorittaminen, mikäli järjestelmän ajo päätyy kaikkien ajettavien suuntauksien osalta (block pathways) resetointi blokkiin tai blokkiin, joka jää odottamaan ulkopuolelta vastausta.
                } else {
                    this.proghmi.sendError(caller + functionname, "BPActions reference was null! Block with UID " + blockUID, -1035, 4, 4);    
                }
            } else {
                // Log or handle the error that the block UID was not found
                this.proghmi.sendError(caller + functionname, "Block with UID " + blockUID + " not found.", -1034, 4, 4);
                retVal=-101; // UID wasn't found
                return retVal;
            }
        }

        // TÄNNE TULEE FUNKTIOT blockstructuresta, JOTKA ON SAATU ACTIONCENTREUI:n ja CONNECTIONSHANDLER:in kautta!

        public void ReadBlockStructureFromAcUI(string kutsuja)
        {
            
        }
    }