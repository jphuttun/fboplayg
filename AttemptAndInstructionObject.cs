using System;
using System.Collections.Generic; // List toiminto löytyy tämän sisästä
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
    
    /// <summary>
    /// Tämä luokka pitää sisällään AltRoute tiedot ja blokkien lukuun liittyvät yksityiskohdat sekä reattempt tapaukset (mutta vielä niitä ei ole toteutettu tämän luokan alla)
    /// This class contains the details of AltRoute and block reading, as well as reattempt cases (but they are not yet implemented under this class)
    /// Unique Identifier: CLS-AAIO-001
    /// </summary>
    public class AttemptAndInstructionObject
    {
        /// <summary>
        /// Tämä lista pitää sisällään UID (long), AttemptObject listan, joka sisältää jokaiselle saman slotin sisällä tapahtuneelle orderille omat parametritietonsa
        /// | This list contains UID (long), a list of AttemptObjects, each containing its own parameter information for each order within the same slot
        /// | Unique Identifier: PRP-AAIO-001
        /// </summary>
        public SortedList<long, AttemptObject> attemptobjects;

        /// <summary>
        /// Tämä indeksi pitää sisällään oman järjestysnumeron (int) sekä sen mukaan talletetut UID indeksit (long) | This index contains its own sequence number (int) and the stored UID indices (long)
        /// Unique Identifier: PRP-AAIO-002
        /// </summary>
        private IOrderIndex<int> ownorderAndUID;

        /// <summary>
        /// Yllä olevan ownorderAndUID kohteen juokseva counteri järjestysnumeroille | Running counter for the above ownorderAndUID
        /// Unique Identifier: PRP-AAIO-003
        /// </summary>
        private int ownorderanduidcounter = 0;

        /// <summary>
        /// Käyttöliittymän referenssi | Reference to the user interface
        /// Unique Identifier: PRP-AAIO-004
        /// </summary>
        private ProgramHMI proghi;

        /// <summary>
        /// Tämän objektin instanssin oma UID arvo | The UID value of this instance
        /// Unique Identifier: PRP-AAIO-005
        /// </summary>
        public long ThisInstanceOwnUID { get; set; }

        /// <summary>
        /// Tämän objektin instanssin vanhemman UID arvo | The UID value of this instance's parent
        /// Unique Identifier: PRP-AAIO-006
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Slotlistarray:n indeksinumero, eli areapointer slotlistaan, jonka slotlist kohteelta tämä objekti löytyy
        /// Index number of Slotlistarray, i.e., areapointer to the slotlist from which this object is found
        /// Unique Identifier: PRP-AAIO-007
        /// </summary>
        private int areapointer = -1;

        /// <summary>
        /// ObjectIndexer luokan referenssi, jonka kautta saadaan annettua/luotua yksittäisille objekteille niiden UID tiedot
        /// Reference to the ObjectIndexer class, which is used to assign/create UID information for individual objects
        /// Unique Identifier: PRP-AAIO-008
        /// </summary>
        private ObjectIndexer objindexerref;

        /// <summary>
        /// Tämä property kertoo onko kohteen toiminta vanhassa moodissa, jolloin sillä voi olla vain yksi AltRoute tieto (jos tämä muuttuja on 0) ja useita yhtäaikaisia AltRoute tietoja, jos tämä muuttuja on 1
        /// Unique Identifier: PRP-AAIO-009
        /// </summary>
        public int IsOldMode { get; private set; }
       
        /// <summary>
        /// Tämä enumeration kertoo minkä tyyppisesti luokkamme käyttäytyy - vanhassa moodissa vai uudessa moodissa, eli voika sillä olla vain yksi AltRoute tieto (jos tämä muuttuja on 0) ja useita yhtäaikaisia AltRoute tietoja, jos tämä muuttuja on 1
        /// </summary>
        public enum attemptInstructionsOldMode {
            /// <summary> Vain yksi AltRoute tieto sallitaan ja tällöin luokka toimii vanhassa moodissa </summary>
            OLD_MODE_IS_SINGLE_ALTROUTE_0=0,

            /// <summary> Useat yhtäaikaiset AltRoute tiedot sallitaan ja tällöin luokka toimii uudessa moodissa </summary>
            OLD_MODE_IS_MULTIPLE_ALTROUTE_1=1
        };

        /// <summary>
        /// Constructor - luodaan luokan instanssi, joka pystyy säilyttämään ohjeet sille, miten yksittäisen slotin tulisi käyttäytyä rakennetun blokkijärjestelmän puitteissa sekä säilyttää listan yrityksistä (attempts), joita on tehty slotin sisällä
        /// Constructor - creates an instance of the class capable of retaining instructions on how a single slot should behave within the constructed block system and maintain a list of attempts made within the slot
        /// Unique Identifier: MTH-AAIO-001
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the path of the caller calling this particular function</param>
        /// <param name="kayref">ProgramHMI, käyttöliittymän referenssi | ProgramHMI, reference to the user interface</param>
        /// <param name="parentuid">long, tämän objektin äitiobjektin UID | long, UID of this object's parent object</param>
        /// <param name="objindex">ObjectIndexer, luokan referenssi, jonka kautta saadaan annettua/luotua yksittäisille objekteille niiden UID tiedot | ObjectIndexer, class reference used to assign/create UID information for individual objects</param>
        /// <param name="slotlistarrayindex">int, sen slotlist luokan instanssin indeksi, jolla kyseinen slotlist luokan kohde löytyy slotlistarray listasta, jonka listan yksittäisen slotin alle tämänkin objektin instanssi kuuluu | int, the index of the slotlist class instance where this slotlist object is found in the slotlistarray list, under which this object's instance also belongs</param>
        /// <param name="isOldMode"> int, tämä muuttuja kertoo onko kohteen toiminta vanhassa moodissa, jolloin sillä voi olla vain yksi AltRoute tieto (jos tämä muuttuja on 0) ja useita yhtäaikaisia AltRoute tietoja, jos tämä muuttuja on 1 </param>
        /// <returns>{void}</returns>
        public AttemptAndInstructionObject(string kutsuja, ProgramHMI kayref, long parentuid, ObjectIndexer objindex, int slotlistarrayindex, int isOldMode)
        {
            string functionname = "->(AAIO)Constructor";
            this.proghi = kayref;
            this.attemptobjects = new SortedList<long, AttemptObject>();
            this.ownorderAndUID = OrderIndexFactory.CreateOrderIndex<int>(kayref);
            this.objindexerref = objindex;
            this.ParentUID = parentuid;
            this.areapointer = slotlistarrayindex;
            this.IsOldMode = isOldMode;
            this.ThisInstanceOwnUID = this.objindexerref.AddObjectToIndexer(kutsuja + functionname, this.ParentUID, (int)ObjectIndexer.indexerObjectTypes.ATTEMPT_AND_INSTRUCTION_OBJECT_1550, slotlistarrayindex); // Register this object as part of the UID-based objects of this program
            if (this.ThisInstanceOwnUID>=0) {
                this.proghi.sendError(kutsuja+functionname,"Fatal error with ObjectIndexer permanentUID:s! Response:"+this.ThisInstanceOwnUID+" ParentUid:"+this.ParentUID,-1075,4,4);
            }            
        }

        /// <summary>
        /// Tämä metodi tyhjää kaikki attemptobjects listan kohteet | This method clears all items in the attemptobjects list
        /// Unique Identifier: MTH-AAIO-002
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the path of the caller calling this particular function</param>
        /// <returns> {void} </returns>
        public void Clear(string kutsuja)
        {
            string functionname = "->(AAIO)ClearAll";

            this.attemptobjects.Clear();
            this.ownorderAndUID.ClearAll(kutsuja+functionname);
        }

        /// <summary>
        /// Tämä metodi resetoi indeksiltään pienimmän attemptobjects listan kohteen. Käytännössä tämä on tapa, jota käytetään silloin kun järjestelmä toimii OldMode==0 (vanha tila), jolloin ei oletettu, että kohde voisi olla samanaikaisesti useassa "tilassa".
        /// Unique Identifier: MTH-AAIO-009
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the path of the caller calling this particular function</param>
        /// <returns> {void} </returns>
        public void ResetSingleAltRoute(string kutsuja)
        {
            string functionname = "->(AAIO)ResetSingleAltRoute";
            long retuid=this.SetSingleAltRoute(kutsuja+functionname,-1); // -1 tässä kuvaa sitä, että AltRoute tieto saa arvon -1;
            if (retuid>=0) {
                if (this.attemptobjects.IndexOfKey(retuid)>-1) { // Tässä luetellaan kaikki muut kohteiden tiedot, jotka haluamme resetoida
                    this.attemptobjects[retuid].AltRouteObjectUID=-1; 
                } else {
                    this.proghi.sendError(functionname, "Object wasn't in attemptobject list: " + retuid, -1016, 4, 4);
                }
            } else {
                this.proghi.sendError(functionname, "Couldn't get UID when reseting altroute: " + retuid, -1015, 4, 4);
            }
        }

        /// <summary>
        /// SetSingleAltRoute method sets the alternate route value (altrouteval) in the attemptobjects list at the position identified by the UID returned from the LowestKeyCorrespondValue method.
        /// If the attemptobjects list is empty, a new AttemptObject is created and added to the list before setting the AltRoute data.
        /// Unique Identifier: MTH-AAIO-006
        /// </summary>
        /// <param name="kutsuja">string, The caller's path that calls this particular function.</param>
        /// <param name="altrouteval">int, The AltRoute value to be set.</param>
        /// <returns> {long} palauttaa kohteen UID arvon, johon tieto asetettiin. Tätä tietoa voidaan käyttää asettamaan samaan objektiin muita tietoja halutulla tavalla. Jos palautettu luku on pienempi kuin 0, kyseessä oli virhe </returns>
        public long SetSingleAltRoute(string kutsuja, int altrouteval)
        {
            string functionname = "->(AAIO)SetSingleAltRoute";
            long retVal=-1;

            // Check if the attemptobjects list is empty and add a new AttemptObject if necessary
            if (this.attemptobjects.Count == 0 || this.ownorderAndUID == null)
            {
                AddAttemptObject(kutsuja + functionname);
            }

            // Retrieve the UID from the LowestKeyCorrespondValue method
            long uid = this.ownorderAndUID.LowestKeyCorrespondValue(kutsuja + functionname);

            // Check if the UID exists in the attemptobjects list and set the AltRoute value
            if (uid >= 0 && attemptobjects.ContainsKey(uid))
            {
                this.attemptobjects[uid].AltRoute = altrouteval;
                retVal=uid;
            }
            else
            {
                // Handle the case where the UID is not found or invalid
                // This part can be replaced with appropriate error handling as needed
                retVal=-2;
                proghi.sendError(kutsuja + functionname, "UID not found or invalid in attemptobjects list: " + uid, -1009, 4, 4);
            }
            return retVal;
        }

        /// <summary>
        /// Palauttaa AltRoute arvon järjestysindeksistä 0. Tätä indeksiä käytetään oletusarvoisesti tietojen säilyttämiseen, mikäli ei useiden yhtäaikaisten yritysten mahdollisuus samassa slotissa ei ole käytössä tai mahdollista
        /// Unique Identifier: MTH-AAIO-005
        /// </summary>
        /// <param name="kutsuja">The caller identifier used in processing.</param>
        /// <returns> {int}, palauttaa AltRoute arvon järjestysindeksistä 0. Jos palautettu arvo pienempi kuin 0, niin kyseessä on virhe. </returns>
        public int GetSingleAltRoute(string kutsuja)
        {
            string functionname="->(AAIO)GetSingleAltRoute";
            int retVal = -4;

            // Tarkistetaan, onko attemptobjects listassa alkioita
            if (attemptobjects.Count > 0 && ownorderAndUID != null) {
                // Etsitään pienin indeksi ownorderAndUID indeksistä
                long minIndex = ownorderAndUID.LowestKeyCorrespondValue(kutsuja+functionname); // Oletetaan, että tämä metodi palauttaa pienimmän indeksin

                // Haetaan AttemptObject tätä indeksiä vastaavalla UID:lla
                if (minIndex >= 0 && attemptobjects.ContainsKey(minIndex)) {
                    retVal = attemptobjects[minIndex].AltRoute;
                } else {
                    retVal = -3; // Ei löydetty sopivaa AttemptObjectia
                }
            } else {
                retVal = -2; // Lista on tyhjä tai ownorderAndUID on null
            }

            return retVal;
        }


        /// <summary>
        /// Tämä metodi lisää uuden AttemptObject:in attemptobjects listalle ja luo sille UID tunnisteen sekä luo sille yksilöllisen sisäisen järjestysnumero indeksin | This method adds a new AttemptObject to the attemptobjects list, creates a UID identifier for it, and creates a unique internal order number index for it
        /// Unique Identifier: MTH-AAIO-003
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param> 
        /// <returns>{long} palauttaa lisätyn kohteen UID numeron. Jos tuli virhe, palauttaa luvun, joka on pienempi kuin 0 | {long} returns the UID number of the added item. If there is an error, returns a number less than 0</returns>
        public long AddAttemptObject(string kutsuja)
        {
            string functionname="->(AAIO)AddAttemptObject";
            long retVal;
            int ordindval=-1;

            retVal = this.objindexerref.AddObjectToIndexer(kutsuja+functionname,this.ThisInstanceOwnUID,(int)ObjectIndexer.indexerObjectTypes.ATTEMPT_OBJECT_1500,this.areapointer); 
            if (retVal>=0) {
                ordindval=this.ownorderAndUID.AddValuesToIndexes(kutsuja+functionname,this.ownorderanduidcounter,retVal,this.areapointer,false,false); // Lisätään kohde erilliseen järjestysluku vs UID listaan
                if (ordindval>=1) {
                    this.attemptobjects.Add(retVal, new AttemptObject(kutsuja+functionname,this.proghi,this.ThisInstanceOwnUID,retVal,ordindval,this.IsOldMode)); // Lisätään kohde listoille
                } else {
                    this.proghi.sendError(kutsuja+functionname,"Couldn't create inverted index to AttemptObject! Response:"+ordindval+" UID:"+retVal,-1004,4,4);
                }
                this.ownorderanduidcounter++;
            } else {
                this.proghi.sendError(kutsuja+functionname,"Couldn't create UID to AttemptObject! Response:"+retVal+" ParentUID:"+this.ThisInstanceOwnUID,-1003,4,4);
            }

            return retVal;
        }

        /// <summary>
        /// Tämä metodi lisää uuden AttemptObjektin attemptobjects listalle ja luo sille UID tunnisteen sekä muut keskeiset tunnistetiedot ja palauttaa objektin täydennettäväksi | This method adds a new AttemptObject to the attemptobjects list, creates a UID identifier and other key identification details for it, and returns the object for completion
        /// Unique Identifier: MTH-AAIO-004
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <returns> {AttemptObject} palauttaa juuri lisätyn AttemptObjektin referenssin. Jos palauttaa null, niin siinä tapauksessa tapahtui virhe! | {AttemptObject} returns a reference to the just added AttemptObject. If it returns null, then an error has occurred!</returns>.
        public AttemptObject AddAttemptObjectAndReturnSelf(string kutsuja)
        {
            string functionname="->(AAIO)AddAttemptObjectAndReturnSelf";
            long uidval=this.AddAttemptObject(kutsuja+functionname);

            if (uidval>=0) {
                int iok=this.attemptobjects.IndexOfKey(uidval);
                if (iok>-1) {
                    return this.attemptobjects[uidval];
                } else {
                    this.proghi.sendError(kutsuja+functionname,"No such AttemptObject in sorted list! IOK:"+iok+" Response:"+uidval,-1006,4,4);
                    return null; 
                }
            } else {
                this.proghi.sendError(kutsuja+functionname,"Couldn't create AttemptObject! Response:"+uidval,-1005,4,4);
                return null;
            }
        }

        /// <summary>
        /// Poistaa AttemptObjectin attemptobjects listalta ja ownorderAndUID indeksistä annetulla UID:lla.
        /// Unique Identifier: MTH-AAIO-007
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <param name="uid">long, UID, joka vastaa poistettavan AttemptObjectin ThisInstanceOwnUID arvoa.</param>
        /// <returns> {int} 2=jos poisto onnistui, 1=jos lista oli tyhjä, 0=jos kohdetta ei ollut listalla, pienempi kuin 0, jos virhe </returns>
        public int RemoveAttemptObjectByUID(string kutsuja, long uid)
        {
            string functionname = "->(AAIO)RemoveAttemptObjectByUID";
            int retVal=-1;
            int amo=this.attemptobjects.Count;

            if (amo>0) {
                if (this.attemptobjects.ContainsKey(uid)) { // Tarkistetaan, onko kyseinen UID attemptobjects listassa
                    
                    int ordindex=this.attemptobjects[uid].OwnOrderIndex;

                    if (ordindex>=0) {
                        if (this.ownorderAndUID.IndexOfKey(kutsuja+functionname,ordindex)>-1) {

                            // Poistetaan UID myös ownorderAndUID indeksistä
                            int succre=this.ownorderAndUID.RemoveValuesFromIndexes(kutsuja+functionname,uid,ordindex); // Tämä metodi poistaa arvon UID ja ordindex tietojen perusteella
                            if (succre>0) {
                                this.attemptobjects.Remove(uid); // Poistetaan AttemptObject attemptobjects listasta
                                retVal=2;
                            } else {
                                retVal=-5;
                                this.proghi.sendError(functionname, "Couldn't remove index! Removal UID " + uid +" OrderIndex:"+ordindex+" Response:"+succre, -1014, 4, 4);
                            }
                        } else {
                            retVal=-4;
                            this.proghi.sendError(functionname, "OwnOrderIndex not in list! Removal UID " + uid +" OrderIndex:"+ordindex, -1013, 4, 4);
                        }
                    } else {
                        retVal=-3;
                        this.proghi.sendError(functionname, "Invalid OwnOrderIndex! Removal UID " + uid +" OrderIndex:"+ordindex, -1012, 4, 4);
                    }
                } else {
                    retVal=-2;
                    proghi.sendError(functionname, "AttemptObject with UID " + uid + " not found in attemptobjects list.", -1009, 4, 4);
                }
            } else {
                retVal=1;
                this.proghi.sendError(functionname, "AttemptObjects list was empty! Removal UID " + uid , -1011, 4, 4);
            }
            return retVal;
        }

        /// <summary>
        /// Poistaa AttemptObjectin attemptobjects listalta ja ownorderAndUID indeksistä annetun järjestysnumeron perusteella.
        /// Unique Identifier: MTH-AAIO-008
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <param name="orderIndex">int, Järjestysnumero, jonka perusteella haetaan poistettavan AttemptObjectin UID.</param>
        /// <returns> {int} 2=jos poisto onnistui, 1=jos lista oli tyhjä, 0=jos kohdetta ei ollut listalla, pienempi kuin 0, jos virhe </returns>
        public int RemoveAttemptObjectByOrderIndex(string kutsuja, int orderIndex)
        {
            string functionname = "->(AAIO)RemoveAttemptObjectByOrderIndex";
            int seektype=0;
            int retVal=-8;

            // Haetaan UID annetulla järjestysnumerolla
            long uid = this.ownorderAndUID.ReturnUniqueIdNum(kutsuja+functionname, orderIndex,seektype); // Oletetaan, että tämä metodi palauttaa UID:n järjestysnumeron perusteella

            if (uid >= 0) {
                retVal=this.RemoveAttemptObjectByUID(kutsuja+functionname,uid); // Käytetään edellä määriteltyä metodia poistamiseen
            } else {
                retVal=-9;
                proghi.sendError(functionname, "No UID found for the given order index: " + orderIndex, -1010, 4, 4);
            }
            return retVal;
        }

        /// <summary>
        /// Hakee ja palauttaa AttemptObject-instanssin annetun UID:n perusteella. Jos kohdetta ei löydy, palauttaa null ja antaa virheilmoituksen.
        /// Retrieves and returns an AttemptObject instance based on the given UID. If the object is not found, returns null and issues an error message.
        /// Unique Identifier: MTH-AAIO-010
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku | string, the caller's path</param>
        /// <param name="uid">long, AttemptObjectin UID arvo jolla etsitään kyseistä attemptobjectia | long, the UID value</param>
        /// <returns>{AttemptObject} Palauttaa AttemptObjectin jos löytyy; muuten null | Returns an AttemptObject if found; otherwise null</returns>
        public AttemptObject GetAttemptObjectByUID(string kutsuja, long uid) {
            string functionname = "->(AAIO)GetAttemptObjectByUID";
            if (attemptobjects.TryGetValue(uid, out AttemptObject obj)) {
                return obj;
            } else {
                proghi.sendError(kutsuja+functionname, "AttemptObject with UID " + uid + " not found.", -1021, 4, 4);
                return null;
            }
        }

        /// <summary>
        /// Hakee ja palauttaa AttemptObject-instanssin annetulla järjestysnumerolla. Jos kohdetta ei löydy, palauttaa null ja antaa virheilmoituksen.
        /// Retrieves and returns an AttemptObject instance based on the given order index. If the object is not found, returns null and issues an error message.
        /// Unique Identifier: MTH-AAIO-011
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku | string, the caller's path</param>
        /// <param name="orderIndex">int, järjestysnumero joka on asetettu ownorderAndUID IOrderIndex indeksiluokkaan | int, the order index</param>
        /// <returns>{AttemptObject} Palauttaa AttemptObjectin jos löytyy; muuten null | Returns an AttemptObject if found; otherwise null</returns>
        public AttemptObject GetAttemptObjectByOrderIndex(string kutsuja, int orderIndex) {
            string functionname = "->(AAIO)GetAttemptObjectByOrderIndex";
            long uid = ownorderAndUID.ReturnUniqueIdNum(kutsuja, orderIndex, (int)ImportantProgramParams.OrderIndexSeekingType.SEEKING_TYPE_EQUAL_VALUE_0);
            if (uid >= 0) {
                if (attemptobjects.TryGetValue(uid, out AttemptObject obj)) {
                    return obj;
                } else {
                    proghi.sendError(kutsuja+functionname, "No AttemptObject found for order index: " + orderIndex+" Response:"+uid, -1023, 4, 4);
                    return null;                    
                }
            } else {
                proghi.sendError(kutsuja+functionname, "Error during seek UID! Order index: " + orderIndex+" Response:"+uid, -1022, 4, 4);
                return null;
            }
        }        

    }

    /// <summary>
    /// Tämä luokka säilyttää eri yrityskertojen alt route kohteiden lukuja (AltRoute) sekä niihin liittyvien objektien UID koodeja (AltRouteObjectUID)
    /// Unique Identifier: CLS-AOBJ-001
    /// </summary>
    public class AttemptObject
    {
        /// <summary>
        /// Käyttöliittymän referenssi
        /// </summary>
        private ProgramHMI proghi;

        private int _altroute;

        /// <summary>
        /// Tätä propertyä on mahdollista käyttää vain silloin kun meillä on IsOldMode==0 - muulloin on käytettävä eri metodeja
        /// Onko vaihtoehtoista reittiä osto/myynti tapahtumille (vastakkainen tuotto), -1=ei käytetty, 0=normaali proseduuri käynnissä, 1=kaksi ostopistettä luotu (ylös ja alas), 2=alempi myyntipiste toteutunut, 3=alempi triggerpiste alitettu, 4=alempi ostopiste luotu, 5=ylempi ostopiste luotu, joka on sama kuin SoldBought=2 HUOM! Nykyään paljon mahdollisia vapaasti määriteltäviä variaatioita!
        /// Indicates alternative routes for buy/sell transactions (opposite yield), -1=not used, 0=normal procedure ongoing, 1=two buying points created (up and down), 2=lower selling point realized, 3=lower trigger point exceeded, 4=lower buying point created, 5=upper buying point created, same as SoldBought=2 NOTE! Nowadays, many possible freely definable variations!
        /// Unique Identifier: PRP-AOBJ-001
        /// </summary>
        public int AltRoute {
            get {
                string functionname="PRP-AO-AltRoute-Get";
                if (this.IsOldMode==(int)AttemptAndInstructionObject.attemptInstructionsOldMode.OLD_MODE_IS_SINGLE_ALTROUTE_0) {
                    return this._altroute; 
                } else {
                    this.proghi.sendError(functionname, "Getting altroute value failed! Wrong type of IsOldMode compared to used function!" , -1017, 4, 4);
                    return -99; // Palauttaa negatiivisen luvun tietona siitä, että tiedon asetus epäonnistui
                }
            }
            set {
                string functionname="PRP-AO-AltRoute-Set";
                if (this.IsOldMode==(int)AttemptAndInstructionObject.attemptInstructionsOldMode.OLD_MODE_IS_SINGLE_ALTROUTE_0) { 
                    this._altroute=value; 
                } else {
                    this.proghi.sendError(functionname, "Setting altroute value failed! Wrong type of IsOldMode compared to used function!" , -1018, 4, 4);
                }
            }
        }

        private long _altrouteobjectuid;

        /// <summary>
        /// AltRoute muuttujaa vastaava blokin UID tieto, jossa kyseinen slotti on sillä hetkellä menossa, mikäli meillä on käytössä ActionCentre ja sillä luotu blokkikonstruktio. Tätä propertyä on mahdollista käyttää vain silloin kun meillä on IsOldMode==0 - muulloin on käytettävä eri metodeja
        /// Corresponding block UID information for the AltRoute variable, where the respective slot is currently progressing, if we are using ActionCentre and the block construction created by it
        /// Unique Identifier: PRP-AOBJ-002
        /// </summary>
        public long AltRouteObjectUID { 
            get {
                string functionname="PRP-AO-AltRouteObjectUID-Get";
                if (this.IsOldMode==(int)AttemptAndInstructionObject.attemptInstructionsOldMode.OLD_MODE_IS_SINGLE_ALTROUTE_0) {
                    return this._altrouteobjectuid;
                } else {
                    this.proghi.sendError(functionname, "Getting altrouteobjectuid value failed! Wrong type of IsOldMode compared to used function!" , -1019, 4, 4);
                    return -98; // Palauttaa negatiivisen luvun tietona siitä, että tiedon asetus epäonnistui
                }
            } 
            set {
                string functionname="PRP-AO-AltRouteObjectUID-Set";
                if (this.IsOldMode==(int)AttemptAndInstructionObject.attemptInstructionsOldMode.OLD_MODE_IS_SINGLE_ALTROUTE_0) {
                    this._altrouteobjectuid=value;
                } else {
                    this.proghi.sendError(functionname, "Setting altrouteobjectuid value failed! Wrong type of IsOldMode compared to used function!" , -1020, 4, 4);
                }
            } 
        }

        /// <summary>
        /// Tämän objektin instanssin oma UID arvo | The UID value of this instance
        /// Unique Identifier: PRP-AOBJ-003
        /// </summary>
        public long ThisInstanceOwnUID { get; set; }

        /// <summary>
        /// Tämän objektin instanssin vanhemman UID arvo | The UID value of this instance's parent
        /// Unique Identifier: PRP-AOBJ-004
        /// </summary>
        public long ParentUID { get; set; }

        /// <summary>
        /// Tämän objektin orderindex IOrderIndex:llä luodussa listassa | This object's order index in the list created by IOrderIndex
        /// Unique Identifier: PRP-AOBJ-005
        /// </summary>
        public int OwnOrderIndex { get; set; }

        /// <summary>
        /// Hallinnoi AltRouteStep-olioita ja niiden vastaavuuksia
        /// Unique Identifier: PRP-AOBJ-006
        /// </summary>
        public AltRouteHandler AlternativeRouteHandler;

        /// <summary>
        /// int, Tämä muuttuja kertoo onko kohteen toiminta vanhassa moodissa, jolloin sillä voi olla vain yksi AltRoute tieto (jos tämä muuttuja on 0) ja useita yhtäaikaisia AltRoute tietoja, jos tämä muuttuja on 1
        /// </summary>
        public int IsOldMode { get; private set; }

        /// <summary>
        /// Constructor - luodaan luokan instanssi, joka pystyy säilyttämään tiedon, kuinka mones yrityskerta kyseinen toiminne on tässä kyseisessä slotissa ja sen lisäksi säilyttämään tiedon missä kaikissa blokkikonstruktion kohteissa koodin ajo on sillä hetkellä
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <param name="prhm"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="parentuid">long, Tämän objektin instanssin vanhemman UID arvo | The UID value of this instance's parent</param>
        /// <param name="ownuid"> Tämän objektin instanssin oma UID arvo | The UID value of this instance </param>
        /// <param name="ownorderind">int, Tämän objektin orderindex IOrderIndex:llä luodussa listassa | This object's order index in the list created by IOrderIndex</param>
        /// <param name="isoldmode"> int, Tämä muuttuja kertoo onko kohteen toiminta vanhassa moodissa, jolloin sillä voi olla vain yksi AltRoute tieto (jos tämä muuttuja on 0) ja useita yhtäaikaisia AltRoute tietoja, jos tämä muuttuja on 1 </param>
        /// <returns> {void} </returns>
        public AttemptObject (string kutsuja, ProgramHMI prhm, long parentuid, long ownuid, int ownorderind, int isoldmode)
        {
            string functionname="->(AO)Constructor";
            this.proghi=prhm;
            this.ParentUID=parentuid;
            this.ThisInstanceOwnUID=ownuid;
            this.OwnOrderIndex=ownorderind;
            this.OwnOrderIndex=ownorderind;
            this.IsOldMode=isoldmode;
            this.AlternativeRouteHandler = new AltRouteHandler(kutsuja+functionname);
        }

        /// <summary>
        /// Tämä metodi tyhjää tämän luokan sellaiset muuttujat jotka tulee nollautua samalla, kuin ennen nollautui AltRoute | This method clears those variables of this class that need to be reset, as previously was done with AltRoute
        /// Unique Identifier: MTH-AOBJ-001
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <returns> {void} </returns>
        public void ResetAttemptObj(string kutsuja)
        {
            this.AltRoute=-1;
            this.AltRouteObjectUID=-1;
        }
               
    }

    /// <summary>
    /// Hallinnoi AltRouteStep-olioita ja niiden vastaavuuksia. Sisältää indeksoinnit int altRoute, AltRouteStep sekä long UID, int altRoute
    /// Manages AltRouteStep objects and their correspondences.
    /// Unique Identifier: CLS-AROH-001
    /// </summary>
    public class AltRouteHandler
    {
        /// <summary>
        /// Kokoelma AltRouteStep instansseja, jotka ovat listassa AltRoute tiedon mukaisesti (Key), joka on tyyppiä int
        /// </summary>
        public Dictionary<int, AltRouteStep> altRoutes;
        
        /// <summary>
        /// Käänteinen indeksi, jossa avaimena on AltRouteObjectUID (tyyppiä long) arvo ja valuena vastaava AltRoute arvo (tyyppiä int)
        /// </summary>
        public Dictionary<long, int> reverseIndex;

        /// <summary>
        /// Tämän luokan paikallinen muuttuja, jolla voidaan käydä läpi AlternativeRouteHandler luokan kokoelma yksi kohde kerrallaan
        /// </summary>
        private int elementpointer=0;

        /// <summary>
        /// Tämän luokan paikallinen muuttuja, jolla voidaan käydä läpi AlternativeRouteHandler luokan käänteinen kokoelma yksi kohde kerrallaan
        /// </summary>
        private int objuidelementpointer=0;        

        /// <summary>
        /// Constructor - luo AltRouteHandler-instanssin ja alustaa tarvittavat rakenteet.
        /// Constructor - creates an instance of AltRouteHandler and initializes necessary structures.
        /// Unique Identifier: MTH-AROH-001
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku | string, the path of the caller</param>
        /// <returns> {void} </returns>
        public AltRouteHandler(string kutsuja)
        {
            this.altRoutes = new Dictionary<int, AltRouteStep>();
            this.reverseIndex = new Dictionary<long, int>();
        }

        /// <summary>
        /// Lisää uuden AltRouteStep-olion hallintaan. | Adds a new AltRouteStep object for management.
        /// Unique Identifier: MTH-AROH-002
        /// </summary>
        /// <param name="altRoute">int, AltRoute-arvo | int, the AltRoute value</param>
        /// <param name="altRouteObjectUID">long, AltRouteObjectUID-arvo | long, the AltRouteObjectUID value</param>
        /// <returns> {void} </returns>
        public void AddAltRouteStep(int altRoute, long altRouteObjectUID)
        {
            AltRouteStep step = new AltRouteStep(altRoute, altRouteObjectUID);
            altRoutes.Add(altRoute, step);
            reverseIndex.Add(altRouteObjectUID, altRoute);
        }

        /// <summary>
        /// Hakee AltRoute-arvon annetun UID:n perusteella. | Retrieves the AltRoute value based on the given UID.
        /// Unique Identifier: MTH-AROH-003
        /// </summary>
        /// <param name="uid">long, AltRouteStepin UID | long, the UID of the AltRouteStep</param>
        /// <returns>{int}, AltRoute-arvon ja pienemmän kuin 0, jos tapahtui virhe | int, the AltRoute value</returns>
        public int GetAltRouteByUID(long uid)
        {
            if (reverseIndex.TryGetValue(uid, out int altRoute))
            {
                return altRoute;
            }
            return -1; // Or some other error handling
        }

        /// <summary>
        /// Poistaa AltRouteStep-olion annetun AltRoute-arvon perusteella. | Removes the AltRouteStep object based on the given AltRoute value.
        /// Unique Identifier: MTH-AROH-004
        /// </summary>
        /// <param name="altRoute">int, poistettavan AltRoute-arvo | int, the AltRoute value to be removed</param>
        /// <returns> {void} </returns>
        public void RemoveAltRouteStep(int altRoute)
        {
            if (this.altRoutes.ContainsKey(altRoute)==true) {
                reverseIndex.Remove(this.altRoutes[altRoute].AltRouteObjectUID);
                altRoutes.Remove(altRoute);
            }
        }

        /// <summary>
        /// Palauttaa ensimmäisen AltRouteStep-arvon ja alustaa sisäisen osoittimen.
        /// Returns the first AltRouteStep value and initializes the internal pointer.
        /// Unique Identifier: MTH-AOBJ-007
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <returns> {int}, Ensimmäisen AltRoute-arvon tai -1, jos kokoelma on tyhjä. Palauttaa -2, jos tapatui virhe odottamaton virhe. </returns>
        public int FirstAltRouteStep(string kutsuja)
        {
            if (this.altRoutes.Count == 0) {
                return -1;
            } else {
                this.elementpointer=0;
                return this.altRoutes.ElementAt(this.elementpointer).Key;
            }

            return -2;
        }

        /// <summary>
        /// Palauttaa seuraavan AltRouteStep-arvon kokoelmasta.
        /// Returns the next AltRouteStep value from the collection.
        /// Unique Identifier: MTH-AOBJ-008
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <returns> {int} Seuraavan AltRoute-arvon tai -1, jos ei enempää kohteita.</returns>
        public int NextAltRouteStep(string kutsuja)
        {
            this.elementpointer++;
            if (this.elementpointer>=this.altRoutes.Count) {
                return -1;
            } else {
                return this.altRoutes.ElementAt(this.elementpointer).Key;
            }
        }

        /// <summary>
        /// Palauttaa ensimmäisen AltRouteObjectUID-arvon ja alustaa sisäisen osoittimen.
        /// Returns the first AltRouteObjectUID value and initializes the internal pointer.
        /// Unique Identifier: MTH-AOBJ-010
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <returns> {long}, Ensimmäisen AltRouteObjectUID-arvon tai -1, jos kokoelma on tyhjä.</returns>
        public long FirstAltRouteObjUid(string kutsuja)
        {
            if (this.reverseIndex.Count == 0) {
                return -1;
            } else {
                this.objuidelementpointer=0;
                return this.reverseIndex.ElementAt(this.objuidelementpointer).Key;
            }

            return -2;
        }

        /// <summary>
        /// Palauttaa seuraavan AltRouteObjectUID-arvon kokoelmasta.
        /// Returns the next AltRouteObjectUID value from the collection.
        /// Unique Identifier: MTH-AOBJ-009
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota | string, the caller's path that calls this particular function</param>
        /// <returns> {long} Seuraavan AltRouteObjectUID-arvon tai -1, jos ei enempää kohteita.</returns>
        public long NextAltRouteObjUid(string kutsuja)
        {
            this.objuidelementpointer++;
            if (this.objuidelementpointer>=this.reverseIndex.Count) {
                return -1;
            } else {
                return this.reverseIndex.ElementAt(this.objuidelementpointer).Key;
            }
        }         
    }


    /// <summary>
    /// Säilyttää yksittäisen AltRoute-tiedon ja vastaavan AltRouteObjectUID:n.
    /// Stores individual AltRoute information and corresponding AltRouteObjectUID.
    /// Unique Identifier: CLS-AROS-001
    /// </summary>
    public class AltRouteStep
    {
        /// <summary>
        /// Onko vaihtoehtoista reittiä osto/myynti tapahtumille (vastakkainen tuotto), -1=ei käytetty, 0=normaali proseduuri käynnissä, 1=kaksi ostopistettä luotu (ylös ja alas), 2=alempi myyntipiste toteutunut, 3=alempi triggerpiste alitettu, 4=alempi ostopiste luotu, 5=ylempi ostopiste luotu, joka on sama kuin SoldBought=2 HUOM! Nykyään paljon mahdollisia vapaasti määriteltäviä variaatioita!
        /// Indicates alternative routes for buy/sell transactions (opposite yield), -1=not used, 0=normal procedure ongoing, 1=two buying points created (up and down), 2=lower selling point realized, 3=lower trigger point exceeded, 4=lower buying point created, 5=upper buying point created, same as SoldBought=2 NOTE! Nowadays, many possible freely definable variations!
        /// Unique Identifier: PRP-AROS-001
        /// </summary>        
        public int AltRoute { get; private set; }

        /// <summary>
        /// AltRoute muuttujaa vastaava blokin UID tieto, jossa kyseinen slotti on sillä hetkellä menossa, mikäli meillä on käytössä ActionCentre ja sillä luotu blokkikonstruktio
        /// Corresponding block UID information for the AltRoute variable, where the respective slot is currently progressing, if we are using ActionCentre and the block construction created by it
        /// Unique Identifier: PRP-AROS-002
        /// </summary>        
        public long AltRouteObjectUID { get; private set; }

        /// <summary>
        /// Constructor - luo AltRouteStep-instanssin ja asettaa tarvittavat arvot. | Constructor - creates an instance of AltRouteStep and sets necessary values.
        /// Unique Identifier: MTH-AROS-001
        /// </summary>
        /// <param name="altRoute">int, AltRoute-arvo | int, the AltRoute value</param>
        /// <param name="altRouteObjectUID">long, AltRouteObjectUID-arvo | long, the AltRouteObjectUID value</param>
        /// <returns> {void} </returns>
        public AltRouteStep(int altRoute, long altRouteObjectUID)
        {
            AltRoute = altRoute;
            AltRouteObjectUID = altRouteObjectUID;
        }
    }
