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
    /// Tämä rajapinta asettaa kaikki luotavat blokit yhteen muottiin, jotta ne keskeisiltä osin toteuttavat oikeat blokkien ajoon liittyvät kohteet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOperationalBlocks<T> where T : notnull
    {
        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään IOperationalBlock objektin tyyppiä objectindexerin listoilta</param>
        /// <param name="parentuid"> long parent UID numero, jolla etsitään MotherConnectionRectanglen objekti ja sen kautta tarkistetaan onko kaikki incoming Handlet saaneet arvonsa </param>
        /// <returns>{IncomingHandlesStatus} Luokan instanssi, joka pitää sisällään CheckIncomingHandles käskyn palautuksessa listan niitä UID arvoja, jota EIVÄT ole vielä päivittyneet tietojensa tietojensa osalta</returns>
        public IncomingHandlesStatus CheckIncomingHandles(string kutsuja, long UIDtoseek, long parentuid);
        
        /// <summary>
        /// Tarkistaa tulokahvat ja päivittää IncomingHandlesStatus-objektin niiden kahvojen osalta, jotka eivät ole vielä saaneet alkuarvojaan.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle, jonka kahvoja käsitellään.</param>
        /// <param name="incominghandle">IncomingHandlesStatus, johon päivitetään kahvat, jotka eivät ole saaneet arvojaan. Jos tämä on null, niin käytetään objektin omaa this.handleconnuidlist IncomingHandlesStatus instanssia</param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>        
        public IncomingHandlesStatus CheckIncomingHandles(string kutsuja, MotherConnectionRectangle motherconnrect, IncomingHandlesStatus incominghandle=null);

        /// <summary>
        /// Tämä luokan instanssi luo kahvojen BlockAtomValuet ja pitää ne järjestyksessä ja tällä käskyllä palautetaan kyseisen luokan referenssi
        /// </summary>
        public BlockHandles ReturnBlockHandlesRef { get; }

        /// <summary>
        /// ExecuteBlock tekee sen mitä kukin blokki tekee ja toteuttaa kyseisen blokin toiminnan
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="motherconnrect"> MotherConnectionRectangle, se pääblokin luokan instanssin referenssi, jonka kautta saamme käytyä noutamassa tiedot siitä, mitä blokille on luotu</param>
        /// <param name="oneslot"> OneSlot, sen slotin referenssi, josta tietoja "saatetaan" hakea. Kyseisen slotin kautta on myös ObjectIndexerillä mahdollista päästä käsiksi koko ohjelman perusparametreihin. Tämä voidaan antaa myös null tietona, jos kyseessä on käyttäjän itsensä antama arvo, jolloin OneSlot objektin referenssiä ei tarvita </param>
        /// <returns>{int} palauttaa BlockAtomValue:n tyypin enum:in, jos onnistui asettamaan kohteen tälle blokille Result tiedoksi. Jos tulee virhe, niin palauttaa arvon, joka on &lt; 0.</returns>
        public int ExecuteBlock(string kutsuja, MotherConnectionRectangle motherconnrect, OneSlot oneslot=null);                       
    }

    /// <summary>
    /// Tämä abstrakti luokka pitää sisällään kaikki ne käskyt ja ominaisuudet, jotka ovat yhteisiä luotaville aliluokille
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    public abstract class OperationalBlocks<T> : IOperationalBlocks<T> where T : notnull
    {
        /// <summary> Se vaihtoehto, joka on valittu comparisonblock kohteessa Operator comboboxin valinnaksi</summary>
        private int selectedhandle=-1;

        /// <summary> Se vaihtoehto, joka on valittu comparisonblock kohteessa Operator comboboxin valinnaksi</summary>
        public int SelectedHandle {
            get { return this.selectedhandle; }
            protected set { this.selectedhandle=value; }
        }

        /// <summary> int, RouteId, joka vastaa käytännössä AltRoute tietoa Slotlistalla </summary>
        public int RouteId { get; set; }

        private string blokinnimi="";
        /// <summary> string, Blokin nimi / Title - ei käytetä mihinkään, mutta antaa lisätietoa käyttäjälle, miksi blokki on luotu </summary>
        public string BlockName { 
            get { return this.blokinnimi; } 
            set { this.blokinnimi=value; } 
        }

        private long ownuid;
        /// <summary>Tämän objektin oma UID </summary>
        public long OwnUID {
            get { return this.ownuid; }
            protected set { this.ownuid=value; }
        }

        private long parentobjuid;
        /// <summary> This object parent UID </summary>
        public long ParentUID {
            get { return this.parentobjuid; }
            protected set { this.parentobjuid=value; }
        }

        private long granparentobjuid;
        /// <summary> This object granparent UID </summary>
        public long GranParentUID {
            get { return this.granparentobjuid; }
            protected set { this.granparentobjuid=value; }
        }        

        /// <summary> Käyttöliittymän referenssi </summary>
        protected ProgramHMI proghmi;

        /// <summary> Referenssi ObjectIndexer luokkaan joka ylläpitää tietoja minkä tyyppisistä objekteista on kyse ja niiden UID tiedoista sekä objektin instanssin referenssistä </summary>
        protected ObjectIndexer objectindexer;

        /// <summary>
        /// Tämän luokan instanssi kertoo sisäisessä listassaan, mitkä ConnectionUID kohteet eivät ole vielä täyttyneet  
        /// </summary> 
        protected IncomingHandlesStatus handleconnuidlist;

        /// <summary>
        /// Atomi, johon voidaan tallentaa jokin blokin keskeinen tieto ja joka toimitetaan eteenpäin blokkikonstruktiossa. Tämä on kuitenkin vain varattu ExecuteBlock käskyn tarpeisiin. Kahvojen atomit luodaan BlockHandles luokassa.
        /// </summary>
        public BlockAtomValue BlockResultValue;

        /// <summary>
        /// Tämä luokan instanssi luo kahvojen BlockAtomValuet ja pitää ne järjestyksessä
        /// </summary>
        protected BlockHandles blockhandles;

        /// <summary>
        /// Tämä luokan instanssi luo kahvojen BlockAtomValuet ja pitää ne järjestyksessä ja tällä käskyllä palautetaan kyseisen luokan referenssi
        /// </summary>
        public BlockHandles ReturnBlockHandlesRef {
            get { return this.blockhandles;}
        }                     

        /// <summary>
        /// Tämä metodi tarkistaa, löytääkö UIDtoseek UID:lla kyseisestä kohdasta MotherConnectionRectangle tyyppisen olion. Jos löytää, niin palauttaa sen. Jos ei löydä niin palauttaa virheen, tai tuntemattoman virheen, jos homma meni mystisesti pieleen.
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <returns> {MotherConnectionRectangle} Jos metodi löytyy UIDtoseek UID:lla MohterConnectionRectangle tyyppisen olion, niin palauttaa sen. Jos ei löydä niin palauttaa virheen, tai tuntemattoman virheen, jos homma meni mystisesti pieleen.</returns>
        protected MotherConnectionRectangle ReturnMotherConnectionRectangleByUID(string kutsuja, long UIDtoseek)
        {
            string functionname="->(IOB)ReturnMotherConnectionRectangleByUID";
            MotherConnectionRectangle parentmotherrect = this.objectindexer.GetTypedObject<MotherConnectionRectangle>(kutsuja+functionname,UIDtoseek);
            if (parentmotherrect!=null) {
                return parentmotherrect;
            } else {
                if (this.objectindexer.GetLastError!=null) {
                    this.proghmi.sendError(kutsuja+functionname,"Error while returning MotherConnectionRectangle! UID:"+UIDtoseek+" Old message:"+this.objectindexer.GetLastError.WholeErrorMessage, -1186,4,4);
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Error while returning MotherConnectionRectangle! UID:"+UIDtoseek+" Unknown error!", -1187,4,4);
                }
                return null;
            }            
        }

        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa ja suorittaa kahvan lähtöarvojen eteenpäin kopioinnin
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <param name="parentuid"> long parent UID numero, jolla etsitään MotherConnectionRectanglen objekti ja sen kautta tarkistetaan onko kaikki incoming Handlet saaneet arvonsa </param>
        /// <returns>{IncomingHandlesStatus} Luokan instanssi, joka pitää sisällään CheckIncomingHandles käskyn palautuksessa listan niitä UID arvoja, jota EIVÄT ole vielä päivittyneet tietojensa tietojensa osalta</returns>
        protected IncomingHandlesStatus CheckIncomingHandlesBase(string kutsuja, long UIDtoseek, long parentuid)
        {
            string functionname="->(IOB)ChekcIncomingHandlesBase#1";
            IncomingHandlesStatus incomingHandlesStatus = this.CheckIncomingHandlesProtected(kutsuja + functionname, UIDtoseek, parentuid);
            if (incomingHandlesStatus != null) {
                if (incomingHandlesStatus.IsIncomingHandleStatusOkay == (int)IncomingHandlesStatus.handleStatusOkay.HANDLE_STATUS_TRUE_1)
                {
                    MotherConnectionRectangle motherRect = this.objectindexer.GetTypedObject<MotherConnectionRectangle>(kutsuja + functionname, parentuid);
                    if (motherRect != null)
                    {
                        int result = this.CopyBlockAtomValuesFromConnectionRectagleToBlockHandle(kutsuja + functionname, motherRect, incomingHandlesStatus);
                        if (result < 0)
                        {
                            this.proghmi.sendError(kutsuja + functionname, "Failed to copy BlockAtomValues from ConnectionRectangle to BlockHandle. Response: " + result, -1336, 4, 4);
                        }
                    }
                    else
                    {
                        this.proghmi.sendError(kutsuja + functionname, "MotherConnectionRectangle not found for UID: " + parentuid, -1337, 4, 4);
                    }
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "IncomingHandlesStatus is null", -1341, 4, 4);
            }
            return incomingHandlesStatus;
        }

        /// <summary>
        /// Tarkistaa tulokahvat ja päivittää IncomingHandlesStatus-objektin niiden kahvojen osalta, jotka eivät ole vielä saaneet alkuarvojaan. Lisäksi suorittaa kahvan lähtöarvojen eteenpäin kopioinnin.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle, jonka kahvoja käsitellään.</param>
        /// <param name="incominghandle">IncomingHandlesStatus, johon päivitetään kahvat, jotka eivät ole saaneet arvojaan. </param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>
        protected IncomingHandlesStatus CheckIncomingHandlesBase(string kutsuja, MotherConnectionRectangle motherconnrect, IncomingHandlesStatus incominghandle)
        {
            string functionname="->(IOB)CheckIncomingHandlesBase#2";
            IncomingHandlesStatus incomingHandlesStatus = incominghandle ?? this.handleconnuidlist;
            incomingHandlesStatus = this.CheckIncomingHandlesProtected(kutsuja + functionname, motherconnrect, incomingHandlesStatus);
            
            if (incomingHandlesStatus != null) {
                if (incomingHandlesStatus.IsIncomingHandleStatusOkay == (int)IncomingHandlesStatus.handleStatusOkay.HANDLE_STATUS_TRUE_1)
                {
                    int result = this.CopyBlockAtomValuesFromConnectionRectagleToBlockHandle(kutsuja + functionname, motherconnrect, incomingHandlesStatus);
                    if (result < 0)
                    {
                        this.proghmi.sendError(kutsuja + functionname, "Failed to copy BlockAtomValues from ConnectionRectangle to BlockHandle. Response: " + result, -1338, 4, 4);
                    }
                }
            } else {
                this.proghmi.sendError(kutsuja + functionname, "IncomingHandlesStatus is null", -1342, 4, 4);
            }
            return incomingHandlesStatus;
        }

        /// <summary>
        /// Copies BlockAtomValue instances from ConnectionRectangle to OperationalBlock in the defined order.
        /// </summary>
        /// <param name="kutsuja">The caller's path.</param>
        /// <param name="motherRect">MotherConnectionRectangle instance containing the connections.</param>
        /// <param name="incominghandles">IncomingHandlesStatus instance to check the status of incoming handles.</param>
        /// <returns>{int} Returns 2 if successful, 1 if direct reference, but not all incoming handles didn't get values yet, 0 if not all incoming handles didn't get values yet, otherwise a negative error code.</returns>
        public int CopyBlockAtomValuesFromConnectionRectagleToBlockHandle(string kutsuja, MotherConnectionRectangle motherRect, IncomingHandlesStatus incominghandles)
        {
            string functionname = "->(OBF)CopyBlockAtomValuesFromConnectionRectagleToBlockHandle";
            try
            {
                if (incominghandles != null)
                {
                    if (this.ReturnBlockHandlesRef != null)
                    {
                        if (incominghandles.IsIncomingHandleStatusOkay == (int)IncomingHandlesStatus.handleStatusOkay.HANDLE_STATUS_TRUE_1)
                        {
                            // Kopioidaan tulopuolen kahvojen arvot
                            int result = motherRect.CopyAllInputHandlesBlockAtomValues(kutsuja + functionname);
                            if (result < 0)
                            {
                                this.proghmi.sendError(kutsuja + functionname, "Failed to copy input handles for MotherConnectionRectangle UID: " + motherRect.OwnUID + " Response:" + result, -1330, 4, 4);
                                return result;
                            }
                            return 2; // Success, all input handles copied
                        }
                        else if (incominghandles.IsIncomingHandleStatusOkay == (int)IncomingHandlesStatus.handleStatusOkay.HANDLE_STATUS_FALSE_0)
                        {
                            return 0; // Not all incoming handles didn't get values yet
                        } else if (incominghandles.IsIncomingHandleStatusOkay == (int)IncomingHandlesStatus.handleStatusOkay.HANDLE_STATUS_WITH_NO_INCOMING_HANDLES_MINUS_1) {
                            return 2; // Tulopuolen kahvoja ei ole ollenkaan kyseisellä objektilla, joten kaikki on heti kunnossa
                        } else {
                            this.proghmi.sendError(kutsuja + functionname, "Unknown handle status for IncomingHandlesStatus UID: " + incominghandles.OwnUID, -1332, 4, 4);
                            return -1;
                        }
                    }
                    else
                    {
                        this.proghmi.sendError(kutsuja + functionname, "ReturnBlockHandlesRef is null", -1333, 4, 4);
                        return -2;
                    }
                }
                else
                {
                    this.proghmi.sendError(kutsuja + functionname, "IncomingHandlesStatus is null", -1334, 4, 4);
                    return -3;
                }
            }
            catch (Exception ex)
            {
                // Error: Exception occurred
                this.proghmi.sendError(kutsuja + functionname, "Exception: " + ex.Message, -1335, 4, 4);
                return -4;
            }
        }

        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <param name="parentuid"> long parent UID numero, jolla etsitään MotherConnectionRectanglen objekti ja sen kautta tarkistetaan onko kaikki incoming Handlet saaneet arvonsa </param>
        /// <returns>{IncomingHandlesStatus} Luokan instanssi, joka pitää sisällään CheckIncomingHandles käskyn palautuksessa listan niitä UID arvoja, jota EIVÄT ole vielä päivittyneet tietojensa tietojensa osalta</returns>
        public abstract IncomingHandlesStatus CheckIncomingHandles(string kutsuja, long UIDtoseek, long parentuid);
        
        /// <summary>
        /// Tarkistaa tulokahvat ja päivittää IncomingHandlesStatus-objektin niiden kahvojen osalta, jotka eivät ole vielä saaneet alkuarvojaan.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle, jonka kahvoja käsitellään.</param>
        /// <param name="incominghandle">IncomingHandlesStatus, johon päivitetään kahvat, jotka eivät ole saaneet arvojaan. Jos tämä on null, niin käytetään objektin omaa this.handleconnuidlist IncomingHandlesStatus instanssia</param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>        
        public abstract IncomingHandlesStatus CheckIncomingHandles(string kutsuja, MotherConnectionRectangle motherconnrect, IncomingHandlesStatus incominghandle=null);        

        /// <summary>
        /// Tämä metodi ensin etsii kyseessä olevan blokin annetulla UIDtoseek parametrilla ja sen jälkeen tarkistaa blokin sisältä, onko kaikki tulopuolen kahvat saaneet jo lähtöarvonsa
        /// </summary>
        /// <param name="caller">string, the caller's path.</param>
        /// <param name="UIDtoseek">long UID numero, jolla etsitään objektin tyyppiä objectindexerin listoilta</param>
        /// <param name="parentuid"> long parent UID numero, jolla etsitään MotherConnectionRectanglen objekti ja sen kautta tarkistetaan onko kaikki incoming Handlet saaneet arvonsa. Jos tämä parametri on -1, niin käytetään tämän luokan oletus parentuid tietoa - muussa tapauksessa käytetään annettua parentuid tietoa. </param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>
        protected IncomingHandlesStatus CheckIncomingHandlesProtected(string kutsuja, long UIDtoseek, long parentuid=-1)
        {
            string functionname="->(IOB)CheckIncomingHandlesProtected#1";
            long parentmotherrectuid=-1;
            if (parentuid>-1) { // Tarkistetaan käytetäänkö tämän luokan omaa tietoa vai ulkopuolista annettua tietoa, joka lähinnä on virheentarkistusmielessä tarpeellista tietoa
                parentmotherrectuid=parentuid;
                if (parentuid!=this.ParentUID) {
                    this.proghmi.sendError(kutsuja+functionname,"Error with parent UIDs: UID original:"+this.ParentUID+" UID given:"+parentuid,-1190,4,4);
                }
            } else {
                parentmotherrectuid=this.ParentUID;
            }

            if (this.objectindexer.objectlist.IndexOfKey(parentmotherrectuid)>-1) { // Löytyykö kohde listoilta
                MotherConnectionRectangle parentmotherrect = this.ReturnMotherConnectionRectangleByUID(kutsuja+functionname,parentmotherrectuid);
                if (parentmotherrect!=null) {
                    return this.CheckIncomingHandles(kutsuja+functionname,parentmotherrect);
                } else {
                    this.proghmi.sendError(kutsuja+functionname,"Couldn't find MotherConnectionRectangle in UID:"+parentuid,-1189,4,4);
                }
            } else {
                this.proghmi.sendError(kutsuja+functionname,"Couldn't find from objectlist a object with UID:"+parentuid,-1188,4,4);
            }

            return null;
        }

        /// <summary>
        /// Tarkistaa tulokahvat ja päivittää IncomingHandlesStatus-objektin niiden kahvojen osalta, jotka eivät ole vielä saaneet alkuarvojaan.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle, jonka kahvoja käsitellään.</param>
        /// <param name="incominghandle">IncomingHandlesStatus, johon päivitetään kahvat, jotka eivät ole saaneet arvojaan. </param>
        /// <returns>{IncomingHandlesStatus} Palauttaa referenssin IncomingHandlesStatus objektiin, joka pitää yllä kullekin objektille tulokahvoihin saapuvien Connectionien UID listasta, jotka EIVÄT vielä ole saaneet arvoa ja näinollen ExecuteBlock ei ole vielä ajettavissa</returns>
        protected IncomingHandlesStatus CheckIncomingHandlesProtected(string kutsuja, MotherConnectionRectangle motherconnrect, IncomingHandlesStatus incominghandle)
        {
            string functionname = "->(IOB)CheckIncomingHandlesProtected#2";
            IncomingHandlesStatus incomstatus;
            int respo=-1;

            if (incominghandle==null) {
                this.proghmi.sendError(kutsuja + functionname, "IncomingHandle is null", -1275, 4, 4);
                return null;
            } else {
                incominghandle.ClearListOfConnectionUIDs(-1);
                incomstatus=incominghandle;
            }

            if (motherconnrect == null) {
                this.proghmi.sendError(kutsuja + functionname, "MotherConnectionRectangle is null", -1277, 4, 4);
                return null;
            }

            if (incomstatus == null) {
                this.proghmi.sendError(kutsuja + functionname, "IncomingHandlesStatus is null", -1276, 4, 4);
                return null;
            }

            for (int iterationclass=1; iterationclass<=2; iterationclass++) {
                long connectionUID = motherconnrect.smallBoxRectangles.IterateThroughConnectionsFirst(kutsuja+functionname, iterationclass);
                while (connectionUID >= 0) {
                    Connection conn = this.objectindexer.GetTypedObject<Connection>(kutsuja+functionname, connectionUID);
                    if (conn != null) {
                        if (conn.ReturnSendingBlockAtomValueRef.AtomType == (int)BlockAtomValue.AtomTypeEnum.Not_defined_yet) {
                            respo=incomstatus.AddNewUID(kutsuja+functionname, connectionUID, false, (int)IncomingHandlesStatus.reportingType.REPORT_ERROR_ALWAYS);
                        } else {
                            respo=incomstatus.AddNewUID(kutsuja+functionname, connectionUID, true, (int)IncomingHandlesStatus.reportingType.REPORT_ERROR_ALWAYS);
                        }
                        if (respo<0) {
                            this.proghmi.sendError(kutsuja + functionname, "Error to add uid to list! UID: " + connectionUID+" Response:"+respo, -1287, 4, 4);
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Connection is null for UID: " + connectionUID, -1274, 4, 4);
                    }
                    connectionUID = motherconnrect.smallBoxRectangles.IterateThroughConnectionsNext(kutsuja+functionname, iterationclass);
                }
            }

            return incomstatus;
        }

        /// <summary>
        /// ExecuteBlock tekee sen mitä kukin blokki tekee ja toteuttaa kyseisen blokin toiminnan
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="motherconnrect"> MotherConnectionRectangle, se pääblokin luokan instanssin referenssi, jonka kautta saamme käytyä noutamassa tiedot siitä, mitä blokille on luotu</param>
        /// <param name="oneslot"> OneSlot, sen slotin referenssi, josta tietoja "saatetaan" hakea. Kyseisen slotin kautta on myös ObjectIndexerillä mahdollista päästä käsiksi koko ohjelman perusparametreihin. Tämä voidaan antaa myös null tietona, jos kyseessä on käyttäjän itsensä antama arvo, jolloin OneSlot objektin referenssiä ei tarvita </param>
        /// <returns>{int} palauttaa BlockAtomValue:n tyypin enum:in, jos onnistui asettamaan kohteen tälle blokille Result tiedoksi. Jos tulee virhe, niin palauttaa arvon, joka on &lt; 0.</returns>
        public abstract int ExecuteBlock(string kutsuja, MotherConnectionRectangle motherconnrect, OneSlot oneslot=null);

        /// <summary>
        /// Palauttaa SortedList &lt; long, BlockHandle &gt; -muotoisen listan, jossa on BlockHandle instanssien referenssejä.
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="iterationclass">int, iterationclass-arvo, jonka mukaan BlockHandle-instanssit suodatetaan.</param>
        /// <returns>{SortedList &lt; long, BlockHandle &gt; }, joka sisältää suodatetut BlockHandle-instanssit.</returns>
        protected SortedList<long, BlockHandle> GetBlockHandlesByIterationClass(string kutsuja, int iterationclass)
        {
            SortedList<long, BlockHandle> blockHandles = new SortedList<long, BlockHandle>();
            string functionName = "->(OB)GetBlockHandlesByIterationClass";
            
            try
            {
                long handleUID = this.ReturnBlockHandlesRef.ReturnBlockHandleUIDFirst(kutsuja + functionName, iterationclass);
                while (handleUID >= 0)
                {
                    BlockHandle handle = this.ReturnBlockHandlesRef.ReturnBlockHandleByUID(kutsuja + functionName, handleUID, true);
                    if (handle.IterationClass == iterationclass)
                    {
                        blockHandles.Add(handleUID, handle);
                    }
                    else
                    {
                        this.proghmi.sendError(kutsuja + functionName, "Handle iteration class mismatch.", -1371, 4, 4);
                    }
                    handleUID = this.ReturnBlockHandlesRef.ReturnBlockHandleUIDNext(kutsuja + functionName, iterationclass);
                    if (handleUID < -2)
                    {
                        this.proghmi.sendError(kutsuja + functionName, "Invalid handle UID. Response:"+handleUID, -1372, 4, 4);
                    }
                }
            }
            catch (Exception ex)
            {
                this.proghmi.sendError(kutsuja + functionName, ex.Message, -1373, 4, 4);
            }

            return blockHandles;
        }

        /// <summary>
        /// Lähettää tulokset eteenpäin jokaiselle "result" pään Connection instansseille.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect">MotherConnectionRectangle instanssi, josta Connection instanssit haetaan.</param>
        /// <result> {long} palauttaa 1, jos onnistui ja pääsi viimeiseen elementtiin listalla tai 2, jos ei yhtään kohdetta listalla. Palauttaa miinusmerkkisen arvon jos eteen tuli virhe </returns>
        public abstract long SendHandlesForward(string kutsuja, MotherConnectionRectangle motherconnrect);

        /// <summary>
        /// Lähettää tulokset eteenpäin jokaiselle "result" pään Connection instansseille.
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect"> MotherConnectionRectangle instanssi, josta Connection instanssit haetaan.</param>
        /// <param name="blockresultvalue"> BlockAtomValue, se BlockAtomValue instanssi, joka halutaan kopioitavan eteenpäin Connection instanssin sisälle </param>
        /// <result> {long} palauttaa 1, jos onnistui ja pääsi viimeiseen elementtiin listalla tai 2, jos ei yhtään kohdetta listalla. Palauttaa miinusmerkkisen arvon jos eteen tuli virhe </returns>
        protected long SendHandlesForwardProtected(string kutsuja, MotherConnectionRectangle motherconnrect, BlockAtomValue blockresultvalue)
        {
            string functionname = "->(IOB)SendHandlesForward";
            ConnectionRectangles connectionRects = motherconnrect.smallBoxRectangles;
            long connUID = connectionRects.IterateThroughConnectionsFirst(kutsuja, (int)ConnectionRectangles.connectionBoxType.RED_BOX_RESULT_VALUE_3);

            while (connUID >= 0) {
                Connection conn = this.objectindexer.GetTypedObject<Connection>(kutsuja+functionname,connUID);
                if (conn!=null) {
                    conn.ReturnSendingBlockAtomValueRef.CopyFrom(blockresultvalue);
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Failed to get connection by UID: " + connUID+" Errorvalue:"+this.objectindexer.GetLastError.ErrorCode+" ErrorMessage:"+this.objectindexer.GetLastError.WholeErrorMessage, -1271, 4, 4);
                }
                connUID = connectionRects.IterateThroughConnectionsNext(kutsuja, (int)ConnectionRectangles.connectionBoxType.RED_BOX_RESULT_VALUE_3);
            }

            if (connUID<-2) { // Tapahtui jokin virhe, joka palautetaan eteenpäin
                this.proghmi.sendError(kutsuja+functionname,"Error to get Connection UID from list!",-1272,4,4);
            } else if (connUID<0) {
                connUID=connUID*-1; // Muutetaan palautettavaa lukua varten negatiivinen luku positiiviseksi, koska normaalioloissa luku olisi aina -1 tai -2 riippuen päästiinkö listan loppuun vai oliko listassa ylipäätään kohteita
            }

            return connUID;
        }

        /// <summary>
        /// Lähettää tulokset eteenpäin jokaiselle "result" pään Connection instansseille käyttäen BlockHandles instanssia ja iterationclass arvoa. ActiveSynapse tieto on päivitetty BlockAtomValue instansseille, josta voi tarkistaa, tulisiko kyseistä Connecitonia hyödyntää vai ei
        /// </summary>
        /// <param name="kutsuja">Kutsujan polku, joka kutsuu tätä funktiota.</param>
        /// <param name="motherconnrect"> MotherConnectionRectangle instanssi, josta Connection instanssit haetaan. TODO: TÄLLÄ HETKELLÄ HIEMAN TURHA PARAMETRI, mutta voi tulla tarpeeseen lähitulevaisuudessa! </param>
        /// <param name="blockHandles">BlockHandles instanssi, josta BlockHandle instanssit haetaan.</param>
        /// <param name="iterationclass">Iterationclass arvo, jonka mukaan BlockHandle instanssit suodatetaan.</param>
        /// <returns>{long} palauttaa 1, jos onnistui ja pääsi viimeiseen elementtiin listalla tai 2, jos ei yhtään kohdetta listalla. Palauttaa miinusmerkkisen arvon, jos eteen tuli virhe.</returns>
        protected long SendHandlesForwardProtected(string kutsuja, MotherConnectionRectangle motherconnrect, BlockHandles blockHandles, int iterationclass)
        {
            string functionname = "->(IOB)SendHandlesForward";
            long handleUID = blockHandles.ReturnBlockHandleUIDFirst(kutsuja + functionname, iterationclass);

            while (handleUID >= 0) {
                BlockHandle handle = blockHandles.ReturnBlockHandleByUID(kutsuja + functionname, handleUID, true);
                if (handle != null) {
                    long assosiatedObjectUID = handle.AssosiatedObjectUID;
                    ConnectionRectangle connectionRectangle = this.objectindexer.GetTypedObject<ConnectionRectangle>(kutsuja + functionname, assosiatedObjectUID);
                    if (connectionRectangle != null) {
                        foreach (long connectionUID in connectionRectangle.ConnectionUIDs) {
                            Connection conn = this.objectindexer.GetTypedObject<Connection>(kutsuja + functionname, connectionUID);
                            if (conn != null) {
                                conn.ReturnSendingBlockAtomValueRef.CopyFrom(handle.ReturnBlockAtomValueRef);
                            } else {
                                this.proghmi.sendError(kutsuja + functionname, "Failed to get connection by UID: " + connectionUID + " Errorvalue:" + this.objectindexer.GetLastError.ErrorCode + " ErrorMessage:" + this.objectindexer.GetLastError.WholeErrorMessage, -1382, 4, 4);
                            }
                        }
                    } else {
                        this.proghmi.sendError(kutsuja + functionname, "Failed to get connection rectangle by UID: " + assosiatedObjectUID + " Errorvalue:" + this.objectindexer.GetLastError.ErrorCode + " ErrorMessage:" + this.objectindexer.GetLastError.WholeErrorMessage, -1383, 4, 4);
                    }
                } else {
                    this.proghmi.sendError(kutsuja + functionname, "Failed to get BlockHandle by UID: " + handleUID + " Errorvalue:" + this.objectindexer.GetLastError.ErrorCode + " ErrorMessage:" + this.objectindexer.GetLastError.WholeErrorMessage, -1384, 4, 4);
                }
                handleUID = blockHandles.ReturnBlockHandleUIDNext(kutsuja + functionname, iterationclass);
            }

            if (handleUID < -2) { // Tapahtui jokin virhe, joka palautetaan eteenpäin
                this.proghmi.sendError(kutsuja + functionname, "Error to get BlockHandle UID from list! Response:"+handleUID, -1385, 4, 4);
            } else if (handleUID < 0) {
                handleUID = handleUID * -1; // Muutetaan palautettavaa lukua varten negatiivinen luku positiiviseksi, koska normaalioloissa luku olisi aina -1 tai -2 riippuen päästiinkö listan loppuun vai oliko listassa ylipäätään kohteita
            }

            return handleUID;
        }        
    }