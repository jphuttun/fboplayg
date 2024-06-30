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
    /// Tämän luokan avulla käyttäjä voi syöttää blokkiin joko oman arvon tai vaihtoehtoisesti käyttää jotain arvoa, joka saadaan itse järjestelmältä
    /// </summary> 
    public class ValueBlock : OperationalBlocks<ValueBlock>
    {

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

        /// <summary> Luokan constructor, joka vastaa tietojen hakemisesta parametrilistasta tai käyttää käyttäjän antamaa omaa tietoa </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="prohmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="objind">ObjectIndexer, referenssi ObjectIndexer luokkaan joka ylläpitää tietoja minkä tyyppisistä objekteista on kyse ja niiden UID tiedoista sekä objektin instanssin referenssistä </param>
        /// <param name="parentuid"> long, tämän objektin luoneen äitiobjektin UID </param>
        /// <param name="granparentuid"> long, tämän objektin luoneen äitiobjektin UID </param>
        /// <param name="objuid"> long, tämän objektin oma uid </param> 
        /// <param name="valueblockt"> int, minkälainen blokki on kyseessä - onko kyseessä blokki, jolle käyttäjä antaa omat arvonsa vai blokki, jossa haetaan arvo parametrilistasta vai minkätyyppinen value block </param>
        /// <param name="selectedhandl"> int, Se vaihtoehto, joka on valittu comparisonblock kohteessa Operator comboboxin valinnaksi </param>
        /// <param name="route"> int, RouteId, joka vastaa käytännössä AltRoute tietoa Slotlistalla </param>
        /// <param name="blockn"> string, Blokin nimi / Title - ei käytetä mihinkään, mutta antaa lisätietoa käyttäjälle, miksi blokki on luotu </param>
        /// <returns> {void} </returns>
        public ValueBlock(string kutsuja, ProgramHMI prghmi, ObjectIndexer objind, long objuid, long parentuid, long granparentuid, int valueblockt, int selectedhandl, int route, string blockn)
        {
            string functionname="->(VB)ValueBlock";
            this.valueblocktype=valueblockt;
            this.SelectedHandle=selectedhandl;
            this.BlockName=blockn;
            this.RouteId=route;
            this.ParentUID=parentuid;
            this.GranParentUID=granparentuid;
            this.OwnUID=objuid;
            this.proghmi=prghmi;
            this.objectindexer=objind;
            this.BlockResultValue=new BlockAtomValue(); // Luodaan atomi, jota voidaan siirtää blokista toiseen. Tämä atomi on tarkoitettu vain ExecuteBlock toimintoa varten. Kahvojen atomit luodaan BlockHandles luokassa

            if (valueblockt>=(int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100 && valueblockt<(int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_SET_VALUE_150) {
                this.handleconnuidlist=new IncomingHandlesStatus(kutsuja+functionname,this.proghmi,this.objectindexer,this.ParentUID,this.GranParentUID,true); // Näillä blokkityypeillä ei ole tulopuolen kahvoja
            } else {
                this.handleconnuidlist=new IncomingHandlesStatus(kutsuja+functionname,this.proghmi,this.objectindexer,this.ParentUID,this.GranParentUID,false); // Näillä blokkityypeillä sitävastoin on tulopuolen kahvat, mutta ei lähtöpuolen kahvoja
            }

            this.blockhandles=new BlockHandles(kutsuja+functionname, this.OwnUID, this.ParentUID, this.proghmi, this.objectindexer); // Luodaan luokka, joka pitää yllä kahvojen tietoja
        }

        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long, UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <param name="parentuid"> long, parent UID numero, jolla etsitään MotherConnectionRectanglen objekti ja sen kautta tarkistetaan onko kaikki incoming Handlet saaneet arvonsa. Jos tämä parametri on -1, niin käytetään tämän luokan oletus parentuid tietoa - muussa tapauksessa käytetään annettua parentuid tietoa. </param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>
        public override IncomingHandlesStatus CheckIncomingHandles(string kutsuja, long UIDtoseek, long motherrect_parent_uid = -1)
        {
            string functionname = "->(VB)CheckIncomingHandles#1";
            return CheckIncomingHandlesBase(kutsuja+functionname, UIDtoseek, motherrect_parent_uid);
        }

        /// <summary>
        /// Tarkistaa tulokahvat ja päivittää IncomingHandlesStatus-objektin niiden kahvojen osalta, jotka eivät ole vielä saaneet alkuarvojaan.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle, jonka kahvoja käsitellään.</param>
        /// <param name="incominghandle">IncomingHandlesStatus, johon päivitetään kahvat, jotka eivät ole saaneet arvojaan. Jos tämä on null, niin käytetään objektin omaa this.handleconnuidlist IncomingHandlesStatus instanssia</param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>
        public override IncomingHandlesStatus CheckIncomingHandles(string kutsuja, MotherConnectionRectangle motherconnrect, IncomingHandlesStatus incominghandle = null)
        {
            string functionname = "->(VB)CheckIncomingHandles#2";
            return CheckIncomingHandlesBase(kutsuja+functionname, motherconnrect, incominghandle);
        }       

        /// <summary>
        /// Value Blockin tapauksessa ExecuteBlock hakee tiedon ohjelmasta tai käyttää käyttäjän antamaa tietoa
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="motherconnrect"> MotherConnectionRectangle, se pääblokin luokan instanssin referenssi, jonka kautta saamme käytyä noutamassa tiedot siitä, mitä blokille on luotu</param>
        /// <param name="oneslot"> OneSlot, sen slotin referenssi, josta tietoja "saatetaan" hakea. Kyseisen slotin kautta on myös ObjectIndexerillä mahdollista päästä käsiksi koko ohjelman perusparametreihin. Tämä voidaan antaa myös null tietona, jos kyseessä on käyttäjän itsensä antama arvo, jolloin OneSlot objektin referenssiä ei tarvita </param>
        /// <returns>{int} palauttaa BlockAtomValue:n tyypin enum:in, jos onnistui asettamaan kohteen tälle blokille Result tiedoksi. Jos tulee virhe, niin palauttaa arvon, joka on &lt; 0.</returns>
        public override int ExecuteBlock(string kutsuja, MotherConnectionRectangle motherconnrect, OneSlot oneslot = null) {
            string functionname = "->(VB)ExecuteBlock";
            int retVal = -1; // Epämääräinen virhe, jos funktio palauttaa tämän luvun

            if (this.ValueBlockType == (int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_100) {
                retVal = HandleCodeValueBlock100(kutsuja + functionname, motherconnrect, oneslot);
            } else if (this.ValueBlockType == (int)ActionCentre.blockTypeEnum.OWN_VALUE_BLOCK_101) {
                retVal = HandleOwnValueBlock101(kutsuja + functionname, motherconnrect);
            } else if (this.ValueBlockType == (int)ActionCentre.blockTypeEnum.CODE_VALUE_BLOCK_SET_VALUE_150) {
                retVal = HandleCodeValueBlockSetValue150(kutsuja + functionname, motherconnrect, oneslot);
            } else {
                this.proghmi.sendError(kutsuja + functionname, "Unknown ValueBlock type! UID:" + this.OwnUID + " AltRoute:" + this.RouteId + " Blockname:" + this.BlockName, -1208, 4, 4);
                retVal = -14;
            }

            return retVal;
        }        

        /// <summary>
        /// Handles the logic for fetching parameter values for the CODE_VALUE_BLOCK_100 block type.
        /// </summary>
        /// <param name="kutsuja">The caller's path invoking this function.</param>
        /// <param name="motherconnrect">Reference to the MotherConnectionRectangle instance, providing necessary information about the block.</param>
        /// <param name="oneslot">Reference to the OneSlot instance, which may provide information needed to fetch parameter values.</param>
        /// <returns> {int} Returns the BlockAtomValue type enumeration if successful. Returns a negative value if an error occurs.</returns>
        private int HandleCodeValueBlock100(string kutsuja, MotherConnectionRectangle motherconnrect, OneSlot oneslot) {
            string functionname="->(VB)HandleCodeValueBlock100";
            int retVal=-1; // Epämääräinen virhe, jos funktio palauttaa tämän luvun
            long granparentuid=-1;
            SmartBot currsmartbotref=null;
            int valu=-1;

            if (oneslot!=null && motherconnrect!=null) {
                string groupname=motherconnrect.StoredUIcomps.StoredParamValues.SelectedGroup;
                string fetchvalue=motherconnrect.StoredUIcomps.StoredParamValues.SelectedOperator;
                long ouid=oneslot.ObjUniqueRefNum;
                if (ouid>=0) {
                    if (this.objectindexer.objectlist.IndexOfKey(ouid)>-1) {
                        granparentuid=this.objectindexer.objectlist[ouid].GranParentUID;
                        if (granparentuid>=0) {
                            int objtyp=this.objectindexer.ReturnObjectType(kutsuja+functionname,granparentuid);
                            if (objtyp==(int)ObjectIndexer.indexerObjectTypes.SMARTBOT_OBJECT_3) {
                                currsmartbotref=this.objectindexer.GetTypedObject<SmartBot>(kutsuja+functionname,granparentuid); // Palauttaa Smartbot tyyppisen objektin referenssin
                                if (currsmartbotref==null) {
                                    this.proghmi.sendError(kutsuja+functionname, "SmartBot reference is null! ! UID:"+ouid+" GranparentUID:"+granparentuid+" Error number:"+this.objectindexer.GetLastError.ErrorCode+" Error message:"+this.objectindexer.GetLastError.WholeErrorMessage, -1238, 4, 4);
                                    retVal = -15;
                                    return retVal;
                                }
                            } else {
                                this.proghmi.sendError(kutsuja+functionname, "Invalid object type! UID:"+ouid+" GranparentUID:"+granparentuid+" Objtype:"+objtyp, -1239, 4, 4);
                                retVal = -16;
                            }
                        } else {
                            this.proghmi.sendError(kutsuja+functionname, "GranParentUID is less than 0! UID:"+ouid+" GranparentUID:"+granparentuid, -1240, 4, 4);
                            retVal = -17;
                        }
                    } else {
                        this.proghmi.sendError(kutsuja+functionname, "Object index key not found! UID:"+ouid, -1241, 4, 4);
                        retVal = -18;
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname, "ObjUniqueRefNum is less than 0! UID:"+ouid, -1242, 4, 4);
                    retVal = -19;
                }
                if (fetchvalue!="") {
                    if (groupname=="SLOT_VALUES") {
                        valu=OneSlot.GetSlotParamValue(kutsuja+functionname,fetchvalue); // Saadaan parametria vastaava kohde int muotoisena enum tyyppinä
                        if (valu>=0) {
                            retVal=oneslot.SetSlotValueParamToBlockAtomValue(kutsuja+functionname, this.BlockResultValue, valu, oneslot); // Asetetaan tieto BlockResultValue atomiin                                
                        } else {
                            this.proghmi.sendError(kutsuja+functionname,"Couldn't convert fetched to enum value! Value:"+valu+" Fetched:#"+fetchvalue+"# GroupName:#"+groupname+"# UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1206,4,4);
                            retVal=-12;
                        }
                    } else if (groupname=="MAIN_PARAMS") {
                        if (currsmartbotref!=null) {
                            valu=SmartBot.GetEnumValue<SmartBot.MainParams>(kutsuja+functionname,fetchvalue);
                            if (valu>=0) {
                                retVal=currsmartbotref.SetMainParamValueToBlockAtomValue(kutsuja+functionname, this.BlockResultValue, valu, currsmartbotref); // Asetetaan tieto BlockResultValue atomiin                                
                            } else {
                                this.proghmi.sendError(kutsuja+functionname,"Couldn't convert fetched value to MainParams enum! Value:"+valu+" Fetched:#"+fetchvalue+"# GroupName:#"+groupname+"# UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1243,4,4);
                                retVal=-20;
                            }
                        } else {
                            this.proghmi.sendError(kutsuja+functionname,"SmartBot reference is null for MAIN_PARAMS", -1244, 4, 4);
                            retVal = -21;
                        }
                    } else if (groupname=="TRIGGERLIST_PARAMS") {
                        if (currsmartbotref!=null) {
                            valu=SmartBot.GetEnumValue<SmartBot.TriggerlistParams>(kutsuja+functionname,fetchvalue);
                            if (valu>=0) {
                                retVal=currsmartbotref.SetTriggerlistParamValueToBlockAtomValue(kutsuja+functionname, this.BlockResultValue, valu, currsmartbotref); // Asetetaan tieto BlockResultValue atomiin                                
                            } else {
                                this.proghmi.sendError(kutsuja+functionname,"Couldn't convert fetched value to TriggerlistParams enum! Value:"+valu+" Fetched:#"+fetchvalue+"# GroupName:#"+groupname+"# UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1245,4,4);
                                retVal=-22;
                            }
                        } else {
                            this.proghmi.sendError(kutsuja+functionname,"SmartBot reference is null for TRIGGERLIST_PARAMS", -1246, 4, 4);
                            retVal = -23;
                        }           
                    } else if (groupname=="COURSE_INFO") {
                        if (currsmartbotref!=null) {
                            valu=SmartBot.GetEnumValue<SmartBot.CourseInfoParams>(kutsuja+functionname,fetchvalue);
                            if (valu>=0) {
                                retVal=currsmartbotref.SetCourseInfoParamValueToBlockAtomValue(kutsuja+functionname, this.BlockResultValue, valu, currsmartbotref); // Asetetaan tieto BlockResultValue atomiin                                
                            } else {
                                this.proghmi.sendError(kutsuja+functionname,"Couldn't convert fetched value to CourseInfoParams enum! Value:"+valu+" Fetched:#"+fetchvalue+"# GroupName:#"+groupname+"# UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1247,4,4);
                                retVal=-24;
                            }
                        } else {
                            this.proghmi.sendError(kutsuja+functionname,"SmartBot reference is null for COURSE_INFO", -1248, 4, 4);
                            retVal = -25;
                        }                        
                    } else {
                        this.proghmi.sendError(kutsuja+functionname,"No such name in parameter list! Name:#"+groupname+"# UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1205,4,4);
                        retVal=-11;
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Invalid fetch value - EMPTY! Fetched:#"+fetchvalue+"# GroupName:#"+groupname+"# UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1207,4,4);
                    retVal=-13;
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Another important reference was null! UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1204,4,4);
                retVal=-10;
            }
        }

        /// <summary>
        /// Handles the logic for setting parameter values for the OWN_VALUE_BLOCK_101 block type.
        /// </summary>
        /// <param name="kutsuja">The caller's path invoking this function.</param>
        /// <param name="motherconnrect">Reference to the MotherConnectionRectangle instance, providing necessary information about the block.</param>
        /// <returns>{int} Returns the BlockAtomValue type enumeration if successful. Returns a negative value if an error occurs.</returns>
        private int HandleOwnValueBlock101(string kutsuja, MotherConnectionRectangle motherconnrect) {
            if (motherconnrect!=null) {
                string groupname=motherconnrect.StoredUIcomps.StoredParamValues.SelectedGroup;
                string fetchvalue=motherconnrect.StoredUIcomps.StoredParamValues.SelectedOperator;
                int atomTypeValue = SmartBot.GetEnumValue<BlockAtomValue.AtomTypeEnum>(kutsuja+functionname, groupname);

                if (atomTypeValue >= 0) {
                    string atomType = ((BlockAtomValue.AtomTypeEnum)atomTypeValue).ToString().Substring(0, 1); // Get the initial character of the AtomTypeEnum string
                    if (this.proghmi.TestValueType(kutsuja+functionname, atomType, fetchvalue)) {
                        retVal = this.SetOwnParamValueToBlockAtomValue(kutsuja+functionname, this.BlockResultValue, atomTypeValue, fetchvalue); // Asetetaan tieto BlockResultValue atomiin
                    } else {
                        this.proghmi.sendError(kutsuja+functionname, "Invalid value type for fetchvalue! Value:#"+fetchvalue+"# GroupName:#"+groupname+"# UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1249,4,4);
                        retVal = -26;
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname, "Couldn't convert groupname to AtomTypeEnum! GroupName:#"+groupname+"# UID:"+this.OwnUID+" AltRoute:"+this.RouteId+" Blockname:"+this.BlockName,-1250,4,4);
                    retVal = -27;
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname, "MotherConnectionRectangle is null", -1251, 4, 4);
                retVal = -28;
            }
        }

        /// <summary>
        /// Asettaa ohjelman parametreihin tiedon BlockAtomValue:sta.
        /// </summary>
        /// <param name="kutsuja"> Kutsujan polku, joka kutsuu tätä funktiota. </param>
        /// <param name="motherconnrect"> MotherConnectionRectangle, josta tarvittavat tiedot haetaan. </param>
        /// <param name="oneslot"> OneSlot, jonka kautta asetettavat tiedot haetaan. </param>
        /// <returns>{int} Palauttaa 1, jos toimenpide onnistui. Palauttaa negatiivisen luvun, jos toimenpide epäonnistui. </returns>
        private int HandleCodeValueBlockSetValue150(string kutsuja, MotherConnectionRectangle motherconnrect, OneSlot oneslot) {
            string functionname = "->(VB)HandleCodeValueBlockSetValue150";
            int retVal = -1;
            long granparentuid = -1;
            SmartBot currsmartbotref = null;

            if (motherconnrect != null) {
                string groupname = motherconnrect.StoredUIcomps.StoredParamValues.SelectedGroup;
                string fetchvalue = motherconnrect.StoredUIcomps.StoredParamValues.SelectedOperator;
                long ouid = oneslot.ObjUniqueRefNum;

                if (ouid>=0) {
                    if (this.objectindexer.objectlist.IndexOfKey(ouid)>-1) {
                        granparentuid=this.objectindexer.objectlist[ouid].GranParentUID;
                        if (granparentuid>=0) {
                            int objtyp=this.objectindexer.ReturnObjectType(kutsuja+functionname,granparentuid);
                            if (objtyp==(int)ObjectIndexer.indexerObjectTypes.SMARTBOT_OBJECT_3) {
                                currsmartbotref=this.objectindexer.GetTypedObject<SmartBot>(kutsuja+functionname,granparentuid); // Palauttaa Smartbot tyyppisen objektin referenssin
                                if (currsmartbotref==null) {
                                    this.proghmi.sendError(kutsuja+functionname, "SmartBot reference is null! ! UID:"+ouid+" GranparentUID:"+granparentuid+" Error number:"+this.objectindexer.GetLastError.ErrorCode+" Error message:"+this.objectindexer.GetLastError.WholeErrorMessage, -1418, 4, 4);
                                    retVal = -35;
                                    return retVal;
                                }
                            } else {
                                this.proghmi.sendError(kutsuja+functionname, "Invalid object type! UID:"+ouid+" GranparentUID:"+granparentuid+" Objtype:"+objtyp, -1419, 4, 4);
                                retVal = -36;
                            }
                        } else {
                            this.proghmi.sendError(kutsuja+functionname, "GranParentUID is less than 0! UID:"+ouid+" GranparentUID:"+granparentuid, -1420, 4, 4);
                            retVal = -37;
                        }
                    } else {
                        this.proghmi.sendError(kutsuja+functionname, "Object index key not found! UID:"+ouid, -1421, 4, 4);
                        retVal = -38;
                    }
                } else {
                    this.proghmi.sendError(kutsuja+functionname, "ObjUniqueRefNum is less than 0! UID:"+ouid, -1422, 4, 4);
                    retVal = -39;
                }

                long handleUID = this.ReturnBlockHandlesRef.ReturnBlockHandleUIDFirst(kutsuja + functionname, (int)ConnectionRectangles.connectionBoxType.YELLOW_BOX_COMPARE_VALUE_1);

                if (handleUID < 0) {
                    this.proghmi.sendError(kutsuja + functionname, "Invalid handle UID. HandleUID: " + handleUID, -1380, 4, 4);
                    return -29;
                }

                BlockHandle handle = this.ReturnBlockHandlesRef.ReturnBlockHandleByUID(kutsuja + functionname, handleUID);

                if (handle == null) {
                    this.proghmi.sendError(kutsuja + functionname, "Handle is null. HandleUID: " + handleUID, -1381, 4, 4);
                    return -30;
                }

                BlockAtomValue atomValue = handle.ReturnBlockAtomValueRef;

                if (atomValue == null) {
                    this.proghmi.sendError(kutsuja + functionname, "BlockAtomValue is null. HandleUID: " + handleUID, -1382, 4, 4);
                    return -31;
                }

                int atomTypeValue = SmartBot.GetEnumValue<BlockAtomValue.AtomTypeEnum>(kutsuja + functionname, groupname);

                if (atomTypeValue >= 0) {
                    if (fetchvalue != "") {
                        if (groupname == "SLOT_VALUES") {
                            retVal = oneslot.SetBlockAtomValueToSlotValueParam(kutsuja + functionname, atomValue, atomTypeValue, fetchvalue);
                            if (retVal < 0) {
                                this.proghmi.sendError(kutsuja + functionname, "Failed to set slot value. AtomType: " + atomTypeValue + ", FetchValue: " + fetchvalue, -1383, 4, 4);
                            }
                        } else if (groupname == "MAIN_PARAMS") {
                            if (currsmartbotref != null) {
                                retVal = currsmartbotref.SetBlockAtomValueToMainParams(kutsuja + functionname, atomValue, fetchvalue);
                            } else {
                                this.proghmi.sendError(kutsuja + functionname, "SmartBot reference is null for MAIN_PARAMS", -1425, 4, 4);
                            }
                        } else if (groupname == "TRIGGERLIST_PARAMS") {
                            if (currsmartbotref != null) {
                                retVal = currsmartbotref.SetBlockAtomValueToTriggerlistParams(kutsuja + functionname, atomValue, fetchvalue);
                            } else {
                                this.proghmi.sendError(kutsuja + functionname, "SmartBot reference is null for TRIGGERLIST_PARAMS", -1426, 4, 4);
                            }
                        } else if (groupname == "COURSE_INFO") {
                            if (currsmartbotref != null) {
                                retVal = currsmartbotref.SetBlockAtomValueToCourseInfoParams(kutsuja + functionname, atomValue, fetchvalue);
                            } else {
                                this.proghmi.sendError(kutsuja + functionname, "SmartBot reference is null for COURSE_INFO", -1427, 4, 4);
                            }
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "No such name in parameter list! Name:#" + groupname + "# UID:" + this.OwnUID + " AltRoute:" + this.RouteId + " Blockname:" + this.BlockName, -1423, 4, 4);
                            retVal = -34;
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Invalid fetch value - EMPTY! Fetched:#" + fetchvalue + "# GroupName:#" + groupname + "# UID:" + this.OwnUID + " AltRoute:" + this.RouteId + " Blockname:" + this.BlockName, -1424, 4, 4);
                        retVal = -40;
                    }
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Invalid atom type value. GroupName: " + groupname, -1384, 4, 4);
                    retVal = -32;
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "MotherConnectionRectangle is null", -1385, 4, 4);
                retVal = -33;
            }

            return retVal;
        }

        /// <summary>
        /// Lähettää tulokset eteenpäin jokaiselle "result" pään Connection instansseille.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle instanssi, josta Connection instanssit haetaan.</param>
        /// <result> {long} palauttaa 1, jos onnistui ja pääsi viimeiseen elementtiin listalla tai 2, jos ei yhtään kohdetta listalla. Palauttaa miinusmerkkisen arvon jos eteen tuli virhe </returns>
        public override long SendHandlesForward(string kutsuja, MotherConnectionRectangle motherconnrect)
        {
            string functionname = "->(VB)SendHandlesForward";
            return this.SendHandlesForwardProtected(kutsuja+functionname, motherconnrect, this.BlockResultValue);
        }        

        /// <summary>
        /// Asettaa käyttäjän itsensä antaman arvon BlockAtomValue-instanssille.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="blockatom">BlockAtomValue-instanssi, johon arvo asetetaan.</param>
        /// <param name="atomTypeValue">AtomTypeEnum-enumin arvo, joka määrittelee, minkä tyyppinen arvo asetetaan.</param>
        /// <param name="fetchvalue">string-arvo, joka asetetaan BlockAtomValue-instanssille, mutta ennen sitä se yritetään muuttaa atomTypeValue:n määrittämään muotoon</param>
        /// <returns>Palauttaa 1, jos toimenpide onnistui. Palauttaa negatiivisen luvun (sisäisen virhekoodin), jos toimenpide epäonnistui.</returns>
        private int SetOwnParamValueToBlockAtomValue(string kutsuja, BlockAtomValue blockatom, int atomTypeValue, string fetchvalue) {
            int retVal = -70; // Epämääräinen virhe
            string functionname = "->(VB)SetOwnParamValueToBlockAtomValue";

            if (blockatom == null) {
                this.proghmi.sendError(kutsuja + functionname, "BlockAtomValue is null", -1252, 4, 4);
                return -77;
            }

            try {
                BlockAtomValue.AtomTypeEnum atomType = (BlockAtomValue.AtomTypeEnum)atomTypeValue;
                switch (atomType) {
                    case BlockAtomValue.AtomTypeEnum.Int:
                        if (int.TryParse(fetchvalue, out int intResult)) {
                            blockatom.IntAtom = intResult;
                            retVal = 1;
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "Invalid int value: " + fetchvalue, -1253, 4, 4);
                            retVal = -71;
                        }
                        break;
                    case BlockAtomValue.AtomTypeEnum.Long:
                        if (long.TryParse(fetchvalue, out long longResult)) {
                            blockatom.LongAtom = longResult;
                            retVal = 1;
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "Invalid long value: " + fetchvalue, -1254, 4, 4);
                            retVal = -72;
                        }
                        break;
                    case BlockAtomValue.AtomTypeEnum.Decimal:
                        if (decimal.TryParse(fetchvalue, out decimal decimalResult)) {
                            blockatom.DecAtom = decimalResult;
                            retVal = 1;
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "Invalid decimal value: " + fetchvalue, -1255, 4, 4);
                            retVal = -73;
                        }
                        break;
                    case BlockAtomValue.AtomTypeEnum.String:
                        blockatom.StringAtom = fetchvalue;
                        retVal = 1;
                        break;
                    case BlockAtomValue.AtomTypeEnum.Bool:
                        if (bool.TryParse(fetchvalue, out bool boolResult)) {
                            blockatom.BoolAtom = boolResult;
                            retVal = 1;
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "Invalid bool value: " + fetchvalue, -1256, 4, 4);
                            retVal = -74;
                        }
                        break;
                    default:
                        this.proghmi.sendError(kutsuja + functionname, "Unsupported AtomTypeEnum value: " + atomTypeValue, -1257, 4, 4);
                        retVal = -75;
                        break;
                }
            } catch (Exception ex) {
                this.proghmi.sendError(kutsuja + functionname, "Exception: " + ex.Message, -1258, 4, 4);
                retVal = -76;
            }

            return retVal;
        }

    }