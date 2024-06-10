using System;
using System.IO;
using System.Collections;
using System.Collections.Generic; // List toiminto löytyy tämän sisästä
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    /// <summary>
    /// Geneerinen rajapinta OrderIndex-luokalle, joka määrittelee toiminnot käänteisen indeksoinnin hallitsemiseksi.
    /// Tyyppiparametri T määrittelee avaimen tyypin SortedList-rakenteessa.
    /// Tämä luokka voidaan lisätä indeksiksi eri kohteille. Luokka pitää kohteet järjesteyksessä, josta ne on nopea hakea. Luokka tekee käänteisen indeksin, jossa slotvalue on key ja unique number osa indeksoitua objektia. Tämä on kyseisestä luokasta decimal - UID indeksi
    /// </summary>
    public interface IOrderIndex<T> where T : notnull
    {
        /// <summary> Orderindex listan kohteiden määrä </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {int} palauttaa kohteiden lukumäärän invindexlist:ssä </returns>
        int Amount(string caller);

        /// <summary> Palauttaa listan alimman kohteen key valuen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {type T}, palauttaa -1 jos virhe, muussa tapauksessa pienimmän kohteen key arvon </returns>          
        T LowestKeyValue(string caller, bool suppressError = false);

        /// <summary> Palauttaa ylimmän kohteen key valuen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {type T}, palauttaa -1 jos virhe, muussa tapauksessa suurimman kohteen key arvon </returns>        
        T HighestKeyValue(string caller, bool suppressError = false);
        int IndexOfKey(string caller, T seekLevel);

        /// <summary>
        /// Tämä metodi palauttaa listasta pienimmällä indeksiarvolla löytyvän kohteen UID (uniqueidnum) arvon, joka on tyyppiä long
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {long}, palauttaa listasta pienimmällä indeksiarvolla löytyvän kohteen UID arvon </returns>
        public long LowestKeyCorrespondValue(string caller, bool suppressError = false);

        /// <summary>
        /// Tämä metodi palauttaa listasta suurimmalla indeksiarvolla löytyvän kohteen UID (uniqueidnum) arvon, joka on tyyppiä long
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {long}, palauttaa listasta suurimmalla indeksiarvolla löytyvän kohteen UID arvon </returns>
        public long HighestKeyCorrespondValue(string caller, bool suppressError = false);     

        /// <summary> Tämä funktio lisää sortedlist listaan slotin arvon keyksi joka on tyyppiä T ja valueksi se tallentaa OneIndex objektin, johon on tallennettu yksilöllinen uniquenumber sekä areapointerin arvo </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="slotvalue"> type T, slotin arvo, joka lisätään indeksiksi </param>
        /// <param name="uniqnumb"> long, kohdeslotin uniikki referenssi numero </param>
        /// <param name="areap"> int, mihinkä kohteeseen/listaan kyseinen operaatioarvo liittyy </param>
        /// <param name="usenegativeslotvalue"> bool, voidaanko käyttää negatiivisia arvoja slotvalue:n arvona vai ei </param>
        /// <param name="usenegativeUID"> bool, voidaanko käyttää negatiivisia arvoja UID arvoina (uniqnub) vai ei </param>
        /// <returns> {int} jos pienempi kuin 0, niin kyseessä virhekoodi ja koodi kertoo, minkälainen virhe oli kyseessä. Jos=0, niin lisäsi arvot, mutta listassa oli jo sillä slotin arvolla toinen arvo, joka oli identtinen. Jos=1, niin lisäsi kohteen listaan. Jos=2, niin samalla arvolla oli toinen kohde, mutta ei identtinen, joten lisäsi kohteen listaan </returns>        
        int AddValuesToIndexes(string caller, T slotValue, long uniqueNumber, int areaPointer, bool usenegativeslotvalue=false, bool usenegativeUID=false);

        /// <summary>
        /// Tyhjentää kaikki tämän OrderIndex kohteen indeksit | Clears all indexes
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku | string, the caller's path</param>
        /// <returns> {void} </returns>
        public void ClearAll(string kutsuja);        

        /// <summary> Tämä funktio poistaa uniquenum numeroisen kohteen käänteisen indeksin listalta, jota se etsii slotvalue paikasta </summary>
        /// <param name="caller"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="slotvalue"> type T, slotin arvo, jota ollaan poistamassa käänteisistä indekseistä </param>
        /// <param name="uniqueNumber"> long, kohdeslotin uniikki referenssi numero </param>
        /// <returns> {int} 2=jos poisto onnistui, 1=jos lista oli tyhjä, 0=jos kohdetta ei ollut listalla, pienempi kuin 0, jos virhe </returns>
        int RemoveValuesFromIndexes(string caller, long uniqueNumber, T slotValue);

        /// <summary> Tämä funktio käy kaikki indeksilistan kohteet läpi ja etsii UniqueRefNumber kohteesta uniqueidnum vastinetta </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="uniqueidnum"> long, kohdeslotin uniikki referenssi numero, jota etsitään kaikista kohteistä järjestyksessä </param>
        /// <returns> {int} jos pienempi kuin 0, jos virhe (=-10, jos etsi kaikki ja ei löytänyt ja =-20, jos listassa ei ollut yhtään kohdetta) ja jos suurempi tai yhtäsuuri kuin 0, niin kohteen indeksi listassa </returns>        
        int SeekIndexByUniqueId(string caller, long uniqueIdNumber);

        /// <summary> Tämä funktio palauttaa Key - Value parin Valuesta uniqueidnumin indeksin perusteella </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="invlistindex"> int, käänteisen indekserin indeksi, jota kohdetta etsitään </param>
        /// <returns> {int} jos pienempi kuin 0, jos virhe ja jos suurempi tai yhtäsuuri kuin 0, niin löydetyn kohteen uniqueidnum </returns>        
        long ReturnUniqueIdByIndex(string caller, int indexListIndex);

        /// <summary> Tämä funktio palauttaa Key - Value parin Valuesta areapointerin indeksin perusteella </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="invlistindex"> int, käänteisen indekserin indeksi, jota kohdetta etsitään </param>
        /// <returns> {int}, pienempi kuin 0, jos virhe ja suurempi tai yhtäsuuri kuin 0, niin löydetyn kohteen AreaPointer </returns>        
        int ReturnAreaPointerByIndex(string caller, int indexListIndex);

        /// <summary> Tämä funktio palauttaa kohteen key arvon, silloin kun kohteen indeksi listassa on annettu </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="invlistindex"> int, käänteisen indekserin indeksi, jota kohdetta etsitään </param>
        /// <returns> {type T} Palauttaa key:n arvon niin monennesta elementistä, kuin indexListIndex järjestysnumero on annettu, eli käytännössä: returnValue=this.invindexlist.ElementAt(indexListIndex).Key; Jos luku pienempi kuin 0, niin virhe </returns>
        T ReturnKeyByIndex(string caller, int indexListIndex);
        
        /// <summary> Tämä funktio palauttaa uniqueidnumberin tästä käänteisestä indeksilistasta, jossa seekval on sen slotin arvo, jota etsitään </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="seekval"> type T, arvo, jota vastaavaa vastinlukua etsitään tästä indeksistä </param>
        /// <param name="seektype"> int, 0=etsitään nimenomaan sitä arvoa, joka on seekval muuttujassa, 1=etsitään sitä arvoa joka on seekval muuttujassa tai pienempää arvoa, mutta kuitenkin suurempaa kuin seuraavaksi pienin arvo, 2=etsitään sitä arvoa, joka on seekval muuttujasa, mutta kuitenkin suurempi kelpuutetaan, jos silti pienempi kuin seuraavaksi suurin arvo </param>
        /// <param name="useforcedareap"> int, pakotetaan löydetystä kohteesta tarkistamaan, onko areapointer myös sama, kuin parametriin annettu ennen kohteen palauttamista käyttäjälle. Jos pienempi kuin 0, niin ei käytetä tätä tarkistusta </param>
        /// <returns> {long}, jos suurempi tai yhtäsuuri kuin 0 palauttaa löydetyn kohteen uniqueidnum (UID) arvon, -1=kohde ei löytynut, -2 ja -3 = kohde löytyi, mutta ei ollut sama useforcedareap parametri, joten palautettiin negatiivinen luku, -4 = listan pituus 0, 
        /// -6 = jotain hämärää, kun kohde meni listan läpi, eikä reagoinut, jos seektype=1 ja seekval pienempi kuin alin slotin arvo, niin palauttaa -21 ja jos seektype=2 ja seekval suurempi kuin ylin slotin arvo, niin palauttaa -22
        /// </returns>   
        long ReturnUniqueIdNum(string caller, T seekValue, int seekType = 0, int useForcedAreaPointer = -1);

        /// <summary> Tämä funktio etsii slottia vastaavan indeksoidun luvun, joka on juuri seeklevel arvon yläpuolella tai sama kuin seeklevel silloin kun aboveindex==true ja sen kohteen joka on juuri seeklevel arvon alapuolella tai sama kuin seeklevel arvo silloin kun aboveindex==false </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="seeklevel"> type T, indeksistä haettava arvo, jota vastaavaa slottiarvoa etsitään </param>
        /// <param name="aboveindex"> bool, jos true, niin palauttaa sen indeksin, joka on kyseistä seeklevel hakuparametria suurempi seuraava slotin indeksi. Jos false, niin tällöin pienempi indeksi. Molemmilla arvoilla palauttaa kohteen indeksin, mikäli seeklevel on täysin sama kuin listasta löydetty Key arvo. </param>
        /// <returns> {int} palauttaa sen indeksin tämän luokan käännetynindeksin listasta, jossa slottiarvo on sama tai pienempi, kuin seeklevel. Jos nykykurssi pienempi kuin alin slottiarvo, niin palauttaa -1, jos käännetyssä indeksilistassa ei ole yhtään slottiarvon vastinetta, palauttaa -4, jos aboveindex=true ja seeklevel yli suurimman arvon, niin palauttaa -10 </returns>        
        int FindInvertedIndex(string caller, T seekLevel, bool aboveIndex = true);
    }

    /// <summary>
    /// Tehdasmetodi, jolla voi luoda erilaisia indeksointi instansseja jotka täyttävät rajapinnan IOrderIndex määreet, eli decimal - UID, long - UID tai string - UID tyyppisiä indeksointeja
    /// </summary>
    public static class OrderIndexFactory
    {
        /// <summary>
        /// Tehdasmetodi, jolla luodaan IOrderIndex tyyppisiä indeksointiobjekteja, jotka täyttävät rajapinnan IOrderIndex määreet, eli decimal - UID, int - UID, long - UID tai string - UID tyyppisiä indeksointeja
        /// </summary>
        /// <typeparam name="T">decimal, long = parametri, jonka tyyppisiä indeksointi kohteita tehdasmetodilla voidaan tehdä, kun toinen indeksoinnin kohde (value arvo) on aina long tyyppinen UID </typeparam>
        /// <param name="hmireference"> ProgramHMI, Käyttöliittymän referenssi </param>
        /// <returns> {IOrderIndex} , palauttaa IOrderIndex tyyppisen indeksointi luokan, jonka tyyppi on T, eli tässä tapauksessa avaimen tyyppi on decimal, long tai string </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IOrderIndex<T> CreateOrderIndex<T>(ProgramHMI hmireference) where T : notnull
        {
            // Tässä tapauksessa, koska haluamme decimal-tyyppisen indeksoinnin,
            // luodaan OrderIndexDecimal-luokan instanssi.
            // Voit lisätä logiikkaa tehdasmetodiin, jos haluat päättää dynaamisesti,
            // mikä aliluokan instanssi luodaan perustuen esimerkiksi `T`:n tyyppiin.
            if (typeof(T) == typeof(decimal)) {
                return (IOrderIndex<T>)(object)new OrderIndexDecimal(hmireference);
            } else if (typeof(T) == typeof(long)) {
                return (IOrderIndex<T>)(object)new OrderIndexLong(hmireference);
            } else if (typeof(T) == typeof(int)) {
                return (IOrderIndex<T>)(object)new OrderIndexInt(hmireference);                
            } else {
                // Heitä poikkeus tai käsittele tilanne, jos T ei ole tuettu tyyppi
                throw new InvalidOperationException("Unsupported type parameter for OrderIndex.");
            }
        }
    }    

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">decimal=indeksoidut kohteet ovat desimaalilukuja, int=indeksoidut kohteet ovat kokonaislukuja Int, long=indeksoidut kohteet ovat kokonaislukuja Long, string=indeksoidut kohteet ovat stringejä</typeparam>
    public abstract class OrderIndex<T> : IOrderIndex<T> where T : notnull
    {
        protected int registeredType;
        private ProgramHMI prhmi;
        protected SortedList<T, OneIndex> invindexlist;

        /// <summary>
        /// Constructor - tämä luokka voidaan alustaa erillisen indeksin, jolle voidaan lisätä alkuperäisen aspektin lisäksi muita aspekteja
        /// </summary>
        /// <param name="prograhmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <returns> {void} </returns>
        public OrderIndex(ProgramHMI prograhmi)
        {
            this.prhmi = prograhmi;
            this.invindexlist = new SortedList<T, OneIndex>();
        }

        /// <summary>
        /// {Int}, tyypit, joita tämän luokan indeksoinneista voi periyttää. 0=indeksoidut kohteet ovat desimaalilukuja, 1=indeksoidut kohteet ovat kokonaislukuja Int, 2=indeksoidut kohteet ovat kokonaislukuja Long, 3=indeksoidut kohteet ovat stringejä
        /// </summary>
        public enum OrderIndexType {
            INDEX_TYPE_DECIMAL_0=0,
            INDEX_TYPE_INT_1=1,
            INDEX_TYPE_LONG_2=2,
            INDEX_TYPE_STRING_3=3
        };

        /// <summary> Orderindex listan kohteiden määrä </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <returns> {int} palauttaa kohteiden lukumäärän invindexlist:ssä </returns>
        public int Amount(string kutsuja)
        {
            int amo=this.invindexlist.Count;
            return amo;
        }

        /*
        public int Amount(string caller)
        {
            return this.invindexlist.Count;
        }
        */

        public abstract T LowestKeyValue(string caller, bool suppressError = false);

        public abstract T HighestKeyValue(string caller, bool suppressError = false);

        public abstract int IndexOfKey(string caller, T seekLevel);

        public abstract int AddValuesToIndexes(string caller, T slotValue, long uniqueNumber, int areaPointer, bool usenegativeslotvalue=false, bool usenegativeUID=false);

        /// <summary> Tämä funktio poistaa uniquenum numeroisen kohteen käänteisen indeksin listalta, jota se etsii slotvalue paikasta </summary>
        /// <param name="caller"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="slotvalue"> type T, slotin arvo, jota ollaan poistamassa käänteisistä indekseistä </param>
        /// <param name="uniqueNumber"> long, kohdeslotin uniikki referenssi numero </param>
        /// <returns> {int} 2=jos poisto onnistui, 1=jos lista oli tyhjä, 0=jos kohdetta ei ollut listalla, pienempi kuin 0, jos virhe </returns>
        public abstract int RemoveValuesFromIndexes(string caller, long uniqueNumber, T slotValue);

        /// <summary>
        /// Tämä metodi palauttaa listasta pienimmällä indeksiarvolla löytyvän kohteen UID (uniqueidnum) arvon, joka on tyyppiä long
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {long], palauttaa listasta pienimmällä indeksiarvolla löytyvän kohteen UID arvon </returns>
        public long LowestKeyCorrespondValue(string caller, bool suppressError = false)
        {
            string funimi="->(OI)LowestKeyCorrespondValue";
            long retVal=-1;
            int amo=this.invindexlist.Count;
            if (amo>0) {
                retVal=ReturnUniqueIdByIndex(caller+funimi,0);
            } else {
                if (suppressError==false) this.prhmi.sendError(caller+funimi,"No key->value pairs in invindexlist!",-1007,2,4);
            }
            return retVal;
        }

        /// <summary>
        /// Tämä metodi palauttaa listasta suurimmalla indeksiarvolla löytyvän kohteen UID (uniqueidnum) arvon, joka on tyyppiä long
        /// </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {long], palauttaa listasta suurimmalla indeksiarvolla löytyvän kohteen UID arvon </returns>
        public long HighestKeyCorrespondValue(string caller, bool suppressError = false)
        {
            string funimi="->(OI)HighestKeyCorrespondValue";
            long retVal=-1;
            int amo=this.invindexlist.Count;
            if (amo>0) {
                retVal=ReturnUniqueIdByIndex(caller+funimi,amo-1);
            } else {
                if (suppressError==false) this.prhmi.sendError(caller+funimi,"No key->value pairs in invindexlist!",-1008,2,4);
            }
            return retVal;
        }        

        /// <summary> Tämä funktio käy kaikki indeksilistan kohteet läpi ja etsii UniqueRefNumber kohteesta uniqueidnum vastinetta </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="uniqueidnum"> long, kohdeslotin uniikki referenssi numero, jota etsitään kaikista kohteistä järjestyksessä </param>
        /// <returns> {int} jos pienempi kuin 0, jos virhe (=-10, jos etsi kaikki ja ei löytänyt ja =-20, jos listassa ei ollut yhtään kohdetta) ja jos suurempi tai yhtäsuuri kuin 0, niin kohteen indeksi listassa </returns>
        public int SeekIndexByUniqueId(string kutsuja, long uniqueidnum)
        {
            int retVal=-10;
            int amo=this.invindexlist.Count;
            int i=0;

            if (amo>0) {
                for (i=0; i<amo; i++) {
                    if (this.invindexlist.ElementAt(i).Value.UniqueRefNumber==uniqueidnum) {
                        retVal=i;
                        return retVal;
                    }
                }
            } else { 
                retVal=-20; 
            }
            return retVal;
        }          

        /// <summary> Tämä funktio palauttaa Key - Value parin Valuesta uniqueidnumin indeksin perusteella </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="invlistindex"> int, käänteisen indekserin indeksi, jota kohdetta etsitään </param>
        /// <returns> {int} jos pienempi kuin 0, jos virhe ja jos suurempi tai yhtäsuuri kuin 0, niin löydetyn kohteen uniqueidnum </returns>
        public long ReturnUniqueIdByIndex(string kutsuja, int invlistindex)
        {
            long retVal=-1;
            int amo=this.invindexlist.Count;

            if (amo>0) { // Onko kohteita listassa enemmän kuin 0
                if (invlistindex<amo) { // Onko indeksi listan alueella
                    retVal=this.invindexlist.ElementAt(invlistindex).Value.UniqueRefNumber;
                } else retVal=-3;
            } else retVal=-2;

            return retVal;
        }

        /// <summary> Tämä funktio palauttaa Key - Value parin Valuesta areapointerin indeksin perusteella </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="invlistindex"> int, käänteisen indekserin indeksi, jota kohdetta etsitään </param>
        /// <returns> {int}, pienempi kuin 0, jos virhe ja suurempi tai yhtäsuuri kuin 0, niin löydetyn kohteen AreaPointer </returns>
        public int ReturnAreaPointerByIndex(string kutsuja, int invlistindex)
        {
            int retVal=-1;
            int amo=this.invindexlist.Count;

            if (amo>0) { // Onko kohteita listassa enemmän kuin 0
                if (invlistindex<amo) { // Onko indeksi listan alueella
                    retVal=this.invindexlist.ElementAt(invlistindex).Value.AreaPointer;
                } else retVal=-3;
            } else retVal=-2;

            return retVal;
        }

        /// <summary>
        /// Tyhjentää kaikki tämän OrderIndex kohteen indeksit | Clears all indexes
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku | string, the caller's path</param>
        /// <returns> {void} </returns>
        public void ClearAll(string kutsuja)
        {
            if (this.invindexlist.Count>0) {
                this.invindexlist.Clear();
            }
            this.invindexlist.TrimExcess();
        }

        /*
        public int SeekIndexByUniqueId(string caller, long uniqueIdNumber)
        {
            // Toteutus esimerkiksi käyttäen LINQ-kyselyä
            var entry = invIndexList.FirstOrDefault(kvp => kvp.Value.UniqueRefNumber == uniqueIdNumber);
            if (!entry.Equals(default(KeyValuePair<T, OneIndex>)))
            {
                return invIndexList.IndexOfKey(entry.Key);
            }
            return -1; // Ei löytynyt
        }

        public long ReturnUniqueIdByIndex(string caller, int indexListIndex)
        {
            if (indexListIndex >= 0 && indexListIndex < invIndexList.Count)
            {
                return invIndexList.Values[indexListIndex].UniqueRefNumber;
            }
            return -1; // Virheellinen indeksi
        }

        public int ReturnAreaPointerByIndex(string caller, int indexListIndex)
        {
            if (indexListIndex >= 0 && indexListIndex < invIndexList.Count)
            {
                return invIndexList.Values[indexListIndex].AreaPointer;
            }
            return -1; // Virheellinen indeksi
        }
        */

        public abstract T ReturnKeyByIndex(string caller, int indexListIndex);

        /// <summary> Tämä funktio palauttaa uniqueidnumberin tästä käänteisestä indeksilistasta, jossa seekval on sen slotin arvo, jota etsitään </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="seekval"> type T, arvo, jota vastaavaa vastinlukua etsitään tästä indeksistä </param>
        /// <param name="seektype"> int, 0=etsitään nimenomaan sitä arvoa, joka on seekval muuttujassa, 1=etsitään sitä arvoa joka on seekval muuttujassa tai pienempää arvoa, mutta kuitenkin suurempaa kuin seuraavaksi pienin arvo, 2=etsitään sitä arvoa, joka on seekval muuttujasa, mutta kuitenkin suurempi kelpuutetaan, jos silti pienempi kuin seuraavaksi suurin arvo </param>
        /// <param name="useforcedareap"> int, pakotetaan löydetystä kohteesta tarkistamaan, onko areapointer myös sama, kuin parametriin annettu ennen kohteen palauttamista käyttäjälle. Jos pienempi kuin 0, niin ei käytetä tätä tarkistusta </param>
        /// <returns> {long}, jos suurempi tai yhtäsuuri kuin 0 palauttaa löydetyn kohteen uniqueidnum (UID) arvon, -1=kohde ei löytynut, -2 ja -3 = kohde löytyi, mutta ei ollut sama useforcedareap parametri, joten palautettiin negatiivinen luku, -4 = listan pituus 0, 
        /// -6 = jotain hämärää, kun kohde meni listan läpi, eikä reagoinut, jos seektype=1 ja seekval pienempi kuin alin slotin arvo, niin palauttaa -21 ja jos seektype=2 ja seekval suurempi kuin ylin slotin arvo, niin palauttaa -22
        /// </returns>
        public abstract long ReturnUniqueIdNum(string caller, T seekValue, int seekType = 0, int useForcedAreaPointer = -1);

        public abstract int FindInvertedIndex(string caller, T seekLevel, bool aboveIndex = true);
    }
 

    /// <summary> Tämä luokka voidaan lisätä indeksiksi eri kohteille. Luokka pitää kohteet järjesteyksessä, josta ne on nopea hakea. Luokka tekee käänteisen indeksin, jossa slotvalue on key ja unique number osa indeksoitua objektia. Tämä on kyseisestä luokasta decimal - UID indeksi </summary>
    public class OrderIndexDecimal : OrderIndex<decimal>
    {
        /// <summary>
        /// Tyypit, joita tämän luokan indeksoinneista voi periyttää. 0=indeksoidut kohteet ovat desimaalilukuja, 1=indeksoidut kohteet ovat kokonaislukuja Int, 2=indeksoidut kohteet ovat kokonaislukuja Long, 3=indeksoidut kohteet ovat stringejä
        /// </summary>
        private int registeredtype;

        /// <summary> Referenssi käyttöliittymään </summary>
        private ProgramHMI prhmi; 
        
        /// <summary> Inverted indeksi, eli ensin on kurssin arvo (Key) ja sitten OneIndex luokka sisältää areapointerin sekä uniikinid numeron </summary> 
        //private SortedList<decimal, OneIndex> invindexlist;

        //private OneIndex ehkarefoneindex;

        /// <summary>
        /// Constructor - tämä luokka voidaan alustaa erillisen indeksin, jolle voidaan lisätä alkuperäisen aspektin lisäksi muita aspekteja
        /// </summary>
        /// <param name="prograhmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <returns> {void} </returns>
        public OrderIndexDecimal(ProgramHMI prograhmi) : base(prograhmi) { 
            this.prhmi = prograhmi;
            this.registeredType = (int)OrderIndexType.INDEX_TYPE_DECIMAL_0;
        }

        /*
        public OrderIndexDecimal(ProgramHMI prograhmi, int registerAsDec=0)
        {
            this.prhmi=prograhmi; // Otetaan käyttöliittymän referenssi talteen

            if (registerAsDec>=0 && registerAsDec<=1) { // TODO: molempien indeksiarvojen yhtäaikainen hyödyntäminen ei vielä mahdollista!
                this.registeredtype=registerAsDec; // 0=indeksoidut kohteet ovat desimaalilukuja, 1=indeksoidut kohteet ovat kokonaislukuja, 2=indeksoidut kohteet ovat stringejä  // TODO: TÄMÄ TOIMINTO EI VIELÄ OLE KÄYTÖSSÄ
            } else {
                this.registeredtype=0; // Indeksoidut kohteet ovat desimaalilukuja
            }

            this.invindexlist = new SortedList<decimal, OneIndex>(); // Luodaan indeksikohteiden lista
        }
        */

        /// <summary> Palauttaa alimman kohteen key valuen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {decimal}, palauttaa -1 jos virhe, muussa tapauksessa pienimmän kohteen key arvon </returns> 
        public override decimal LowestKeyValue(string kutsuja, bool suppresserror=false)
        {
            string funimi="->(OID)LowestKeyValue";
            decimal retVal=-1;

            if (this.Amount(kutsuja+funimi)>0) {
                retVal=this.invindexlist.ElementAt(0).Key;
            } else {
                if (suppresserror==false) this.prhmi.sendError(kutsuja+funimi,"No key->value pairs in invindexlist!",-183,2,4);
            }
            return retVal;
        }

        /// <summary> Palauttaa suurimman kohteen key valuen - TODO: epäily, että tämä käsky ei kertoisikaan suurinta arvoa listassa! </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {decimal}, palauttaa -1 jos virhe, muussa tapauksessa suurimman kohteen key arvon </returns>        
        public override decimal HighestKeyValue(string kutsuja, bool suppresserror=false)
        {
            string funimi="->(OID)HighestKeyValue";
            decimal retVal=-1;
            int amo=-1;

            if (this.Amount(kutsuja+funimi)>0) {
                amo=this.Amount(kutsuja+funimi);
                retVal=this.invindexlist.ElementAt(amo-1).Key;
            } else {
                if (suppresserror==false) this.prhmi.sendError(kutsuja+funimi,"No key->value pairs in invindexlist!",-284,2,4);
            }
            return retVal;
        }

        /// <summary> Tekee saman, kuin IndexOfKey suoraan </summary>
        public override int IndexOfKey(string kutsuja, decimal seeklevel)
        {
            return this.invindexlist.IndexOfKey(seeklevel);
        }

        /// <summary> Tämä funktio lisää sortedlist listaan slotin desimaaliarvon keyksi ja valueksi se tallentaa OneIndex objektin, johon on tallennettu yksilöllinen uniquenumber sekä areapointerin arvo </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="slotvalue"> decimal, slotin arvo, jonka alueella myynti tapahtui </param>
        /// <param name="uniqnumb"> long, kohdeslotin uniikki referenssi numero </param>
        /// <param name="areap"> int, mihinkä kohteeseen/listaan kyseinen operaatioarvo liittyy </param>
        /// <param name="usenegativeslotvalue"> bool, voidaanko käyttää negatiivisia arvoja slotvalue:n arvona vai ei </param>
        /// <param name="usenegativeUID"> bool, voidaanko käyttää negatiivisia arvoja UID arvoina (uniqnub) vai ei </param> 
        /// <returns> {int} jos pienempi kuin 0, niin kyseessä virhekoodi ja koodi kertoo, minkälainen virhe oli kyseessä. Jos=0, niin lisäsi arvot, mutta listassa oli jo sillä slotin arvolla toinen arvo, joka oli identtinen. Jos=1, niin lisäsi kohteen listaan. Jos=2, niin samalla arvolla oli toinen kohde, mutta ei identtinen, joten lisäsi kohteen listaan </returns>
        public override int AddValuesToIndexes(string kutsuja, decimal slotvalue, long uniqnumb, int areap, bool usenegativeslotvalue=false, bool usenegativeUID=false)
        {
            string funimi="->(OID)addValuesToIndexes";
            int retVal=-1;
            int amo=this.invindexlist.Count;
            int indofk=-1;
            bool enterfurtherslotval=false;
            bool enterfurtheruid=false;

            
            if (slotvalue<0 && usenegativeslotvalue==false) {
                enterfurtherslotval=false;
            } else {
                enterfurtherslotval=true;
            }

            if (enterfurtherslotval==true) {
                
                if (uniqnumb<0 && usenegativeUID==false) {
                    enterfurtheruid=false;
                } else {
                    enterfurtheruid=true;
                }

                if (enterfurtheruid==true) {
                    indofk=this.invindexlist.IndexOfKey(slotvalue);
                    if (indofk>=0) { // Kohde on jo listassa
                        if (this.invindexlist.ElementAt(indofk).Value.AreaPointer==areap && this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber==uniqnumb) {
                            this.invindexlist.ElementAt(indofk).Value.AreaPointer=areap;
                            this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber=uniqnumb;

                            this.prhmi.sendError(kutsuja+funimi,"There was already a identical slot in asset list! Replacing values! Slotval:"+slotvalue+" Uniqnum:"+uniqnumb+" Areap:"+areap,-127,2);
                            retVal=0;
                        } else {
                            if (this.invindexlist.ElementAt(indofk).Value.AreaPointer==areap) {
                                this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber=uniqnumb;
                                retVal=2;
                            } else {
                                this.prhmi.sendError(kutsuja+funimi,"There was already a slot in asset list! Adding new copy! Slotval:"+slotvalue+" Uniqnum:"+uniqnumb+" Areap:"+areap+" OldUniqnum:"+this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber+" OldAreap:"+this.invindexlist.ElementAt(indofk).Value.AreaPointer,-128);
                                this.invindexlist.Add(slotvalue,new OneIndex(areap,uniqnumb)); // Lisätään uusi indeksi, jos ei ollut ennestään 
                                retVal=-4;
                            }
                        }
                    } else { // Kohde ei ole listassa, joten lisätään uusi listaan
                        this.invindexlist.Add(slotvalue,new OneIndex(areap,uniqnumb)); // Lisätään uusi indeksi, jos ei ollut ennestään 
                        retVal=1;
                    }
                } else {
                    retVal=-3; // Yksilöllinen numero ei ollut sallituissa rajoissa
                }
            } else retVal=-2; // Slotin numero ei ollut sallituissa rajoissa
        
            return retVal;
        }

        /// <summary> Tämä funktio poistaa uniquenum numeroisen kohteen käänteisen indeksin listalta, jota se etsii slotvalue paikasta </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="uniquenum"> long, kohdeslotin uniikki referenssi numero </param>
        /// <param name="slotvalue"> decimal, slotin arvo, jota ollaan poistamassa käänteisistä indekseistä </param>
        /// <returns> {int} 2=jos poisto onnistui, 1=jos lista oli tyhjä, 0=jos kohdetta ei ollut listalla, pienempi kuin 0, jos virhe </returns>
        public override int RemoveValuesFromIndexes(string kutsuja, long uniquenum, decimal slotvalue)
        {
            int retVal=-1;
            bool succrem=false;
            string funimi="->(OID)RemoveValuesFromIndexes";
            int indexf=-1;
            int iok=-1;

            if (this.invindexlist.Count>0) {
                iok=this.invindexlist.IndexOfKey(slotvalue);
                if (iok>=0) { // Kohde on jo listassa
                    if (this.invindexlist.ElementAt(iok).Value.UniqueRefNumber==uniquenum) { // Tarkistetaan, onko sisässä asetettuna uniikki numero vai ei
                        succrem=this.invindexlist.Remove(slotvalue);
                        if (succrem==false) {
                            retVal=-3;
                            this.prhmi.sendError(kutsuja+funimi,"Unexpected index list removing error! Slotval:"+slotvalue+" Uniqnum:"+uniquenum,-130);
                        } else {
                            retVal=2;
                            invindexlist.TrimExcess();
                        }
                        return retVal;
                    } 
                }

                // Jos ei löytynyt tai uniquenum ei täsmännyt, niin..
                indexf=this.SeekIndexByUniqueId(kutsuja+funimi,uniquenum); // Etsitään kohdetta for loopilla

                if (indexf>=0) { // Kohde löytyi 
                    this.invindexlist.RemoveAt(indexf); // Poistetaan kohde
                    retVal=2;
                } else {
                    if (indexf==-20) { retVal=1; }
                    if (indexf==-10) { retVal=0; }
                }
            } else retVal=1; 

            return retVal;
        }     

        /// <summary> Tämä funktio palauttaa kohteen key arvon (kurssiarvo), silloin kun kohteen indeksi listassa on annettu </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="invlistindex"> int, käänteisen indekserin indeksi, jota kohdetta etsitään </param>        
        public override decimal ReturnKeyByIndex(string kutsuja, int invlistindex)
        {
            decimal retVal=-1;
            int amo=this.invindexlist.Count;

            if (amo>0) { // Onko kohteita listassa enemmän kuin 0
                if (invlistindex<amo) { // Onko indeksi listan alueella
                    retVal=this.invindexlist.ElementAt(invlistindex).Key;
                } else retVal=-3;
            } else retVal=-2;

            return retVal;
        }

        /// <summary> Tämä funktio palauttaa uniqueidnumberin käänteisestä indeksilistasta, jossa seekval on sen slotin arvo, jota etsitään </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="seekval"> decimal, nykykurssin hinta tai muu arvo, jota vastaavaa slottiarvoa etsitään </param>
        /// <param name="seektype"> int, 0=etsitään nimenomaan sitä arvoa, joka on seekval muuttujassa, 1=etsitään sitä arvoa joka on seekval muuttujassa tai pienempää arvoa, mutta kuitenkin suurempaa kuin seuraavaksi pienin arvo, 2=etsitään sitä arvoa, joka on seekval muuttujasa, mutta kuitenkin suurempi kelpuutetaan, jos silti pienempi kuin seuraavaksi suurin arvo </param>
        /// <param name="useforcedareap"> int, pakotetaan löydetystä kohteesta tarkistamaan, onko areapointer myös sama, kuin parametriin annettu ennen kohteen palauttamista käyttäjälle. Jos pienempi kuin 0, niin ei käytetä tätä tarkistusta </param>
        /// <returns> {int}, jos suurempi tai yhtäsuuri kuin 0 palauttaa löydetyn kohteen uniqueidnum arvon, -1=kohde ei löytynut, -2 ja -3 = kohde löytyi, mutta ei ollut sama useforcedareap parametri, joten palautettiin negatiivinen luku, -4 = listan pituus 0, 
        /// -6 = jotain hämärää, kun kohde meni listan läpi, eikä reagoinut, jos seektype=1 ja seekval pienempi kuin alin slotin arvo, niin palauttaa -21 ja jos seektype=2 ja seekval suurempi kuin ylin slotin arvo, niin palauttaa -22
        /// </returns>
        public override long ReturnUniqueIdNum(string kutsuja, decimal seekval, int seektype=0, int useforcedareap=-1)
        {
            string funimi="->(OID)returnUniqueIdNum";
            long retVal=-6;
            int amo=invindexlist.Count;
            bool aboveindx;
            int retind=-1;
            
            if (amo>0) {
                // Etsitään pelkästään kohdearvoa ja palautetaan -1, mikäli kohdetta ei löydy
                if (this.invindexlist.IndexOfKey(seekval)>-1) {
                    if (useforcedareap<0) { // Kohde löytyi, eikä tarvitse tehdä areapointer tarkistusta
                        retVal=this.invindexlist[seekval].UniqueRefNumber;
                        return retVal;
                    } else {
                        if (this.invindexlist[seekval].AreaPointer==useforcedareap) {
                            retVal=this.invindexlist[seekval].UniqueRefNumber;
                            return retVal;                                
                        } else {
                            retVal=-2; // Kohde löytyi, mutta sen areapointer ei ollut sama, kuin useforcedareap parametrissa haettu arvo
                            return retVal; // TODO: HUOM, halutaanko että etsintä keskeytyy tähän, vai halutaanko että etsitään lähistöltä muita vastaavia kohteita, joissa areapointer voisi täsmätä?
                        }
                    }
                }
                if (seektype>0) { // Etsitään kohdearvoa tai sitä suurempaa / pienempää arvoa riippuen, mitä parametriin oli laitettu
                    retVal=-1;
                    if (seektype==1) aboveindx=false; else aboveindx=true; // Tutkitaan, minkälainen hakutapa meillä on kohteelle

                    retind=this.FindInvertedIndex(kutsuja+funimi,seekval,aboveindx); // Etsitään, löytyykö haluttua kohdetta indeksilistasta
                    if (retind>=0) {
                        //this.ehkarefoneindex=this.invindexlist.ElementAt(retind).Value; // Jos löytyy, niin otetaan objekti referenssi talteen
                        if (useforcedareap<0) { // Tehdäänkö pakkotarkistus, oliko areapointer sama - jos <0, niin ei pakkotarkisteta
                            retVal=this.invindexlist.ElementAt(retind).Value.UniqueRefNumber;
                            return retVal;
                        } else { // Tässä tapauksessa tehdään areapointerin suhteen pakkotarkistus
                            if (this.invindexlist.ElementAt(retind).Value.AreaPointer==useforcedareap) {
                                retVal=this.invindexlist.ElementAt(retind).Value.UniqueRefNumber;
                                return retVal;                                
                            } else {
                                retVal=-3; // Kohde löytyi, mutta sen areapointer ei ollut sama, kuin useforcedareap parametrissa haettu arvo
                                return retVal; // TODO: HUOM, halutaanko että etsintä keskeytyy tähän, vai halutaanko että etsitään lähistöltä muita vastaavia kohteita, joissa areapointer voisi täsmätä?
                            }
                        }
                    } else {
                        if (retind==-1 || retind==-10) { // Jos aboveindex=false ja seekval<alin slotin arvo, niin palauttaa -1 ja jos aboveindex=true ja seekval>ylin slotin arvo, niin palauttaa -10
                            if (retind==-1) {
                                retVal=retind-20; // Vanha -1, nyt palauttaa -21
                            } else {
                                retVal=retind-12; // Vanha -10, nyt palauttaa -22
                            }
                            this.prhmi.sendError(kutsuja+funimi,"Seeked value out of list values! Seektype:"+seektype+" Seeklevel:"+seekval+" Retind:"+retind+" RetVal:"+retVal,-339,2,4);
                        } else {
                            retVal=-5;
                            this.prhmi.sendError(kutsuja+funimi,"Error on seeking right index value! Error after seeking:"+retind+" Slotval:"+seekval+" Type:"+seektype+" Areap:"+useforcedareap,-292,4,4);
                        }
                    }
                }
            } else {
                retVal=-4;
            }

            return retVal;
        }

        /// <summary> Tämä funktio etsii slottia vastaavan indeksoidun luvun, joka on juuri nykykurssin hinnan yläpuolella tai sama kuin nykykurssi </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="seeklevel"> decimal, nykykurssin hinta, jota vastaavaa slottiarvoa etsitään </param>
        /// <param name="aboveindex"> bool, jos true, niin palauttaa sen indeksin, joka on kyseistä kurssia suurempi seuraava slotin indeksi. Jos false, niin tällöin pienempi indeksi </param>
        /// <returns> {int} palauttaa sen indeksin tämän luokan käännetynindeksin listasta, jossa slottiarvo on sama tai pienempi, kuin seeklevel. Jos nykykurssi pienempi kuin alin slottiarvo, niin palauttaa -1, jos käännetyssä indeksilistassa ei ole yhtään slottiarvon vastinetta, palauttaa -4, jos aboveindex=true ja seeklevel yli suurimman arvon, niin palauttaa -10 </returns>
        public override int FindInvertedIndex(string kutsuja, decimal seeklevel, bool aboveindex=true)
        {
            int retVal=-3;
            int slotlistcount=-1;
            int mid=-1;
            int apu=-1;
            int minval=-1;
            int maxval=-1;
            int i=0;
            int amo=5;
            int tarval=0;

            slotlistcount=this.invindexlist.Count;

            if (slotlistcount==0) { // Jos kohteita on nolla
                retVal=-4;
            } else {
                if (seeklevel<this.invindexlist.ElementAt(0).Key) { // Jos etsittävä listan pienimmän arvon alapuolella
                    if (aboveindex==false) {
                        retVal=-1; // Jos aboveindex=false ja ollaan alle alimman arvon, niin palautetaan -1
                    } else {
                        retVal=0; // Jos aboveindex=true ja ollaan seeklevelillä alimman arvon alapuolella, niin palautetaan alin indeksi
                    }
                } else { // Etsitään indeksi
                    if (slotlistcount==1) {
                        if (aboveindex==false) { 
                            retVal=0; // Selkeä tilanne, jos kohteita on vain 1
                        } else {
                            if (seeklevel==this.invindexlist.ElementAt(0).Key) {
                                retVal=0; // Jos ainoa indeksi on sama kuin mitä etsitään, niin palautetaan se
                            } else {
                                retVal=-10; // Jos aboveindex=true ja seeklevel on suurempi kuin ainoa indeksi, niin palautetaan -10;
                            }
                        }
                    } else { 
                        if (seeklevel>=this.invindexlist.ElementAt(slotlistcount-1).Key) { // Jos etsittävä listan suurimman arvon yläpuolella
                            if (aboveindex==false) {
                                retVal=slotlistcount-1; // Selkeä tilanne, jos seeklevel on suurimman arvon yläpuolella tai sama arvo ja aboveindex=false
                            } else {
                                if (seeklevel==this.invindexlist.ElementAt(slotlistcount-1).Key) {
                                    retVal=slotlistcount-1;
                                } else {
                                    retVal=-10; // Palauttaa -10 arvon, jos aboveindex=true ja arvo on yli suurimman arvon listassa
                                }
                            }   
                        } else {
                            minval=0;
                            mid=slotlistcount/2;
                            maxval=slotlistcount-1;
                            while (true)
                            {
                                if (maxval-minval<amo) { // Jos indeksien erotus on jo pieni, niin käydään loput läpi for loopilla
                                    if (minval+amo>maxval) { // Määritellään minimi ja maksimi indeksien arvot haettavalle alueelle
                                        tarval=maxval;
                                    } else { 
                                        tarval=minval+amo; 
                                    }
                                    if (seeklevel==this.invindexlist.ElementAt(minval).Key) { // Tarkastetaan, ettei arvo ole haettavan kokonaisuuden alarajalla
                                        retVal=minval;
                                        break;
                                    } else {
                                        for (i=minval; i<tarval; i++) { // Käydään indeksit läpi minimirajalta maksimirajalle -1, koska sitten if lauseessa käytetään i+1 indeksiä
                                            if (seeklevel>this.invindexlist.ElementAt(i).Key && seeklevel<=this.invindexlist.ElementAt(i+1).Key) { // Jos löytyy aiotusta välistä
                                                retVal=i+1;
                                                break;
                                            }
                                        }
                                        if (retVal>=0) {
                                            break; // Poistutaan while loopista, jos oikea kohde löytyi for loopin aikana
                                        }
                                    }
                                } else { // Jos indeksien osalta on vielä runsaasti matkaa toisiinsa, niin jatketaan puolitusmenetelmää
                                    if (seeklevel>this.invindexlist.ElementAt(mid).Key) {
                                        minval=mid; // minval muuttuu, maxval pysyy samana
                                    } else {
                                        maxval=mid; // maxval muuttuu, minval pysyy samana
                                    }
                                    apu=(maxval-minval)/2;
                                    mid=apu+minval; // Luodaan uusi mid arvo puolitusmenetelmää varten
                                }
                            }
                            
                            apu=retVal;
                            // Tähän saakka haettu indeksi arvo on oikein, jos on haettu oletuksella aboveindex=true. Nyt muokataan indeksin arvoa, jos aboveindex=false
                            if (aboveindex==false) {
                                if (seeklevel==this.invindexlist.ElementAt(0).Key) {
                                    retVal=0;
                                } else if (seeklevel==this.invindexlist.ElementAt(apu).Key) {
                                    retVal=apu;
                                } else {
                                    retVal=apu-1;
                                }
                            }

                        }
                    }
                }
            }

            return retVal;
        }        
    }

    /// <summary> Tämä luokka voidaan lisätä indeksiksi eri kohteille. Luokka pitää kohteet järjesteyksessä, josta ne on nopea hakea. Luokka tekee käänteisen indeksin, jossa slotvalue on key ja unique number osa indeksoitua objektia. Tämä on kyseisestä luokasta decimal - UID indeksi 
    /// HUOM! Tätä Long muotoista indeksointia ei ole testattu vielä kaikilta osin
    /// </summary>
    public class OrderIndexLong : OrderIndex<long>
    {
        private int registeredtype;

        /// <summary> Referenssi käyttöliittymään </summary>
        private ProgramHMI prhmi; 
        
        /// <summary> Inverted indeksi, eli ensin on kurssin arvo (Key) ja sitten OneIndex luokka sisältää areapointerin sekä uniikinid numeron </summary> 
        //private SortedList<decimal, OneIndex> invindexlist;

        //private OneIndex ehkarefoneindex;

        /// <summary>
        /// Constructor - tämä luokka voidaan alustaa erillisen indeksin, jolle voidaan lisätä alkuperäisen aspektin lisäksi muita aspekteja
        /// </summary>
        /// <param name="prograhmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <param name="registerAsDec"> int, 0=indeksoidut kohteet ovat desimaalilukuja, 1=indeksoidut kohteet ovat kokonaislukuja, 2=indeksoidut kohteet ovat stringejä </param>
        /// <returns> {void} </returns>
        public OrderIndexLong(ProgramHMI prograhmi) : base(prograhmi) { 
            this.prhmi = prograhmi;
            this.registeredType = (int)OrderIndexType.INDEX_TYPE_LONG_2;
        }

        /// <summary> Palauttaa alimman kohteen key valuen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {long}, palauttaa -1 jos virhe, muussa tapauksessa pienimmän kohteen key arvon </returns> 
        public override long LowestKeyValue(string kutsuja, bool suppresserror=false)
        {
            string funimi="->(OIL)LowestKeyValue";
            long retVal=-1;

            if (this.Amount(kutsuja+funimi)>0) {
                retVal=this.invindexlist.ElementAt(0).Key;
            } else {
                if (suppresserror==false) this.prhmi.sendError(kutsuja+funimi,"No key->value pairs in invindexlist!",-1002,2,4);
            }
            return retVal;
        }

        /// <summary> Palauttaa suurimman kohteen key valuen - TODO: epäily, että tämä käsky ei kertoisikaan suurinta arvoa listassa! </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {decimal}, palauttaa -1 jos virhe, muussa tapauksessa suurimman kohteen key arvon </returns>        
        public override long HighestKeyValue(string kutsuja, bool suppresserror=false)
        {
            string funimi="->(OIL)HighestKeyValue";
            long retVal=-1;
            int amo=-1;

            if (this.Amount(kutsuja+funimi)>0) {
                amo=this.Amount(kutsuja+funimi);
                retVal=this.invindexlist.ElementAt(amo-1).Key;
            } else {
                if (suppresserror==false) this.prhmi.sendError(kutsuja+funimi,"No key->value pairs in invindexlist!",-990,2,4);
            }
            return retVal;
        }

        /// <summary> Tekee saman, kuin IndexOfKey suoraan </summary>
        public override int IndexOfKey(string kutsuja, long seeklevel)
        {
            return this.invindexlist.IndexOfKey(seeklevel);
        }

        /// <summary> Tämä funktio lisää sortedlist listaan slotin desimaaliarvon keyksi ja valueksi se tallentaa OneIndex objektin, johon on tallennettu yksilöllinen uniquenumber sekä areapointerin arvo </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="slotvalue"> long, slotin arvo, jonka alueella myynti tapahtui </param>
        /// <param name="uniqnumb"> long, kohdeslotin uniikki referenssi numero </param>
        /// <param name="areap"> int, mihinkä kohteeseen/listaan kyseinen operaatioarvo liittyy </param>
        /// <param name="usenegativeslotvalue"> bool, voidaanko käyttää negatiivisia arvoja slotvalue:n arvona vai ei </param>
        /// <param name="usenegativeUID"> bool, voidaanko käyttää negatiivisia arvoja UID arvoina (uniqnub) vai ei </param>
        /// <returns> {int} jos pienempi kuin 0, niin kyseessä virhekoodi ja koodi kertoo, minkälainen virhe oli kyseessä. Jos=0, niin lisäsi arvot, mutta listassa oli jo sillä slotin arvolla toinen arvo, joka oli identtinen. Jos=1, niin lisäsi kohteen listaan. Jos=2, niin samalla arvolla oli toinen kohde, mutta ei identtinen, joten lisäsi kohteen listaan </returns>
        public override int AddValuesToIndexes(string kutsuja, long slotvalue, long uniqnumb, int areap, bool usenegativeslotvalue=false, bool usenegativeUID=false)
        {
            string funimi="->(OIL)addValuesToIndexes";
            int retVal=-1;
            int amo=this.invindexlist.Count;
            int indofk=-1;
            bool enterfurtherslotval=false;
            bool enterfurtheruid=false;

            
            if (slotvalue<0 && usenegativeslotvalue==false) {
                enterfurtherslotval=false;
            } else {
                enterfurtherslotval=true;
            }

            if (enterfurtherslotval==true) {
                
                if (uniqnumb<0 && usenegativeUID==false) {
                    enterfurtheruid=false;
                } else {
                    enterfurtheruid=true;
                }

                if (enterfurtheruid==true) {
                    indofk=this.invindexlist.IndexOfKey(slotvalue);
                    if (indofk>=0) { // Kohde on jo listassa
                        if (this.invindexlist.ElementAt(indofk).Value.AreaPointer==areap && this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber==uniqnumb) {
                            this.invindexlist.ElementAt(indofk).Value.AreaPointer=areap;
                            this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber=uniqnumb;

                            this.prhmi.sendError(kutsuja+funimi,"There was already a identical slot in asset list! Replacing values! Slotval:"+slotvalue+" Uniqnum:"+uniqnumb+" Areap:"+areap,-997,2,4);
                            retVal=0;
                        } else {
                            if (this.invindexlist.ElementAt(indofk).Value.AreaPointer==areap) {
                                this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber=uniqnumb;
                                retVal=2;
                            } else {
                                this.prhmi.sendError(kutsuja+funimi,"There was already a slot in asset list! Adding new copy! Slotval:"+slotvalue+" Uniqnum:"+uniqnumb+" Areap:"+areap+" OldUniqnum:"+this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber+" OldAreap:"+this.invindexlist.ElementAt(indofk).Value.AreaPointer,-996,4,4);
                                this.invindexlist.Add(slotvalue,new OneIndex(areap,uniqnumb)); // Lisätään uusi indeksi, jos ei ollut ennestään 
                                retVal=-4;
                            }
                        }
                    } else { // Kohde ei ole listassa, joten lisätään uusi listaan
                        this.invindexlist.Add(slotvalue,new OneIndex(areap,uniqnumb)); // Lisätään uusi indeksi, jos ei ollut ennestään 
                        retVal=1;
                    }
                } else {
                    retVal=-3; // Yksilöllinen numero (UID) ei ollut sallituissa rajoissa
                }
            } else retVal=-2; // Slotin numero ei ollut sallituissa rajoissa
        
            return retVal;
        }

        /// <summary> Tämä funktio poistaa uniquenum numeroisen kohteen käänteisen indeksin listalta, jota se etsii slotvalue paikasta </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="uniquenum"> long, kohdeslotin uniikki referenssi numero </param>
        /// <param name="slotvalue"> long, slotin arvo, jota ollaan poistamassa käänteisistä indekseistä </param>
        /// <returns> {int} 2=jos poisto onnistui, 1=jos lista oli tyhjä, 0=jos kohdetta ei ollut listalla, pienempi kuin 0, jos virhe </returns>
        public override int RemoveValuesFromIndexes(string kutsuja, long uniquenum, long slotvalue)
        {
            int retVal=-1;
            bool succrem=false;
            string funimi="->(OIL)removeValuesFromIndexes";
            int indexf=-1;
            int iok=-1;

            if (this.invindexlist.Count>0) {
                iok=this.invindexlist.IndexOfKey(slotvalue);
                if (iok>=0) { // Kohde on jo listassa
                    if (this.invindexlist.ElementAt(iok).Value.UniqueRefNumber==uniquenum) { // Tarkistetaan, onko sisässä asetettuna uniikki numero vai ei
                        succrem=this.invindexlist.Remove(slotvalue);
                        if (succrem==false) {
                            retVal=-3;
                            this.prhmi.sendError(kutsuja+funimi,"Unexpected index list removing error! Slotval:"+slotvalue+" Uniqnum:"+uniquenum,-998);
                        } else {
                            retVal=2;
                            invindexlist.TrimExcess();
                        }
                        return retVal;
                    } 
                }

                // Jos ei löytynyt tai uniquenum ei täsmännyt, niin..
                indexf=this.SeekIndexByUniqueId(kutsuja+funimi,uniquenum); // Etsitään kohdetta for loopilla

                if (indexf>=0) { // Kohde löytyi 
                    this.invindexlist.RemoveAt(indexf); // Poistetaan kohde
                    retVal=2;
                } else {
                    if (indexf==-20) { retVal=1; }
                    if (indexf==-10) { retVal=0; }
                }
            } else retVal=1; 

            return retVal;
        }     

        /// <summary> Tämä funktio palauttaa kohteen key arvon (kurssiarvo), silloin kun kohteen indeksi listassa on annettu </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="invlistindex"> int, käänteisen indekserin indeksi, jota kohdetta etsitään </param>        
        public override long ReturnKeyByIndex(string kutsuja, int invlistindex)
        {
            long retVal=-1;
            int amo=this.invindexlist.Count;

            if (amo>0) { // Onko kohteita listassa enemmän kuin 0
                if (invlistindex<amo) { // Onko indeksi listan alueella
                    retVal=this.invindexlist.ElementAt(invlistindex).Key;
                } else retVal=-3;
            } else retVal=-2;

            return retVal;
        }

        /// <summary> Tämä funktio palauttaa uniqueidnumberin käänteisestä indeksilistasta, jossa seekval on sen slotin arvo, jota etsitään </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="seekval"> long, nykykurssin hinta tai muu arvo, jota vastaavaa slottiarvoa etsitään </param>
        /// <param name="seektype"> int, 0=etsitään nimenomaan sitä arvoa, joka on seekval muuttujassa, 1=etsitään sitä arvoa joka on seekval muuttujassa tai pienempää arvoa, mutta kuitenkin suurempaa kuin seuraavaksi pienin arvo, 2=etsitään sitä arvoa, joka on seekval muuttujasa, mutta kuitenkin suurempi kelpuutetaan, jos silti pienempi kuin seuraavaksi suurin arvo </param>
        /// <param name="useforcedareap"> int, pakotetaan löydetystä kohteesta tarkistamaan, onko areapointer myös sama, kuin parametriin annettu ennen kohteen palauttamista käyttäjälle. Jos pienempi kuin 0, niin ei käytetä tätä tarkistusta </param>
        /// <returns> {int}, jos suurempi tai yhtäsuuri kuin 0 palauttaa löydetyn kohteen uniqueidnum arvon, -1=kohde ei löytynut, -2 ja -3 = kohde löytyi, mutta ei ollut sama useforcedareap parametri, joten palautettiin negatiivinen luku, -4 = listan pituus 0, 
        /// -6 = jotain hämärää, kun kohde meni listan läpi, eikä reagoinut, jos seektype=1 ja seekval pienempi kuin alin slotin arvo, niin palauttaa -21 ja jos seektype=2 ja seekval suurempi kuin ylin slotin arvo, niin palauttaa -22
        /// </returns>
        public override long ReturnUniqueIdNum(string kutsuja, long seekval, int seektype=0, int useforcedareap=-1)
        {
            string funimi="->(OIL)returnUniqueIdNum";
            long retVal=-6;
            int amo=invindexlist.Count;
            bool aboveindx;
            int retind=-1;
            
            if (amo>0) {
                // Etsitään pelkästään kohdearvoa ja palautetaan -1, mikäli kohdetta ei löydy
                if (this.invindexlist.IndexOfKey(seekval)>-1) {
                    if (useforcedareap<0) { // Kohde löytyi, eikä tarvitse tehdä areapointer tarkistusta
                        retVal=this.invindexlist[seekval].UniqueRefNumber;
                        return retVal;
                    } else {
                        if (this.invindexlist[seekval].AreaPointer==useforcedareap) {
                            retVal=this.invindexlist[seekval].UniqueRefNumber;
                            return retVal;                                
                        } else {
                            retVal=-2; // Kohde löytyi, mutta sen areapointer ei ollut sama, kuin useforcedareap parametrissa haettu arvo
                            return retVal; // TODO: HUOM, halutaanko että etsintä keskeytyy tähän, vai halutaanko että etsitään lähistöltä muita vastaavia kohteita, joissa areapointer voisi täsmätä?
                        }
                    }
                }
                if (seektype>0) { // Etsitään kohdearvoa tai sitä suurempaa / pienempää arvoa riippuen, mitä parametriin oli laitettu
                    retVal=-1;
                    if (seektype==1) aboveindx=false; else aboveindx=true; // Tutkitaan, minkälainen hakutapa meillä on kohteelle

                    retind=this.FindInvertedIndex(kutsuja+funimi,seekval,aboveindx); // Etsitään, löytyykö haluttua kohdetta indeksilistasta
                    if (retind>=0) {
                        //this.ehkarefoneindex=this.invindexlist.ElementAt(retind).Value; // Jos löytyy, niin otetaan objekti referenssi talteen
                        if (useforcedareap<0) { // Tehdäänkö pakkotarkistus, oliko areapointer sama - jos <0, niin ei pakkotarkisteta
                            retVal=this.invindexlist.ElementAt(retind).Value.UniqueRefNumber;
                            return retVal;
                        } else { // Tässä tapauksessa tehdään areapointerin suhteen pakkotarkistus
                            if (this.invindexlist.ElementAt(retind).Value.AreaPointer==useforcedareap) {
                                retVal=this.invindexlist.ElementAt(retind).Value.UniqueRefNumber;
                                return retVal;                                
                            } else {
                                retVal=-3; // Kohde löytyi, mutta sen areapointer ei ollut sama, kuin useforcedareap parametrissa haettu arvo
                                return retVal; // TODO: HUOM, halutaanko että etsintä keskeytyy tähän, vai halutaanko että etsitään lähistöltä muita vastaavia kohteita, joissa areapointer voisi täsmätä?
                            }
                        }
                    } else {
                        if (retind==-1 || retind==-10) { // Jos aboveindex=false ja seekval<alin slotin arvo, niin palauttaa -1 ja jos aboveindex=true ja seekval>ylin slotin arvo, niin palauttaa -10
                            if (retind==-1) {
                                retVal=retind-20; // Vanha -1, nyt palauttaa -21
                            } else {
                                retVal=retind-12; // Vanha -10, nyt palauttaa -22
                            }
                            this.prhmi.sendError(kutsuja+funimi,"Seeked value out of list values! Seektype:"+seektype+" Seeklevel:"+seekval+" Retind:"+retind+" RetVal:"+retVal,-1000,2,4);
                        } else {
                            retVal=-5;
                            this.prhmi.sendError(kutsuja+funimi,"Error on seeking right index value! Error after seeking:"+retind+" Slotval:"+seekval+" Type:"+seektype+" Areap:"+useforcedareap,-999,4,4);
                        }
                    }
                }
            } else {
                retVal=-4;
            }

            return retVal;
        }

        /// <summary>
        /// Tämä funktio etsii slottia vastaavan indeksoidun luvun, joka on juuri nykykurssin hinnan yläpuolella tai sama kuin nykykurssi.
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota</param>
        /// <param name="seeklevel">long, nykykurssin hinta, jota vastaavaa slottiarvoa etsitään</param>
        /// <param name="aboveindex">bool, jos true, niin palauttaa sen indeksin, joka on kyseistä kurssia suurempi seuraava slotin indeksi. Jos false, niin tällöin pienempi indeksi</param>
        /// <returns>int palauttaa sen indeksin tämän luokan käännetyn indeksin listasta, jossa slottiarvo on sama tai pienempi, kuin seeklevel. Jos nykykurssi pienempi kuin alin slottiarvo, niin palauttaa -1, jos käännetyssä indeksilistassa ei ole yhtään slottiarvon vastinetta, palauttaa -4, jos aboveindex=true ja seeklevel yli suurimman arvon, niin palauttaa -10</returns>
        public override int FindInvertedIndex(string kutsuja, long seeklevel, bool aboveindex = true)
        {
            int retVal = -3;
            int slotlistcount = -1;
            int mid = -1;
            int apu = -1;
            int minval = -1;
            int maxval = -1;
            int i = 0;
            int amo = 5;
            int tarval = 0;

            slotlistcount = this.invindexlist.Count;

            if (slotlistcount == 0)
            { // Jos kohteita on nolla
                retVal = -4;
            }
            else
            {
                if (seeklevel < this.invindexlist.ElementAt(0).Key)
                { // Jos etsittävä listan pienimmän arvon alapuolella
                    if (aboveindex == false)
                    {
                        retVal = -1; // Jos aboveindex=false ja ollaan alle alimman arvon, niin palautetaan -1
                    }
                    else
                    {
                        retVal = 0; // Jos aboveindex=true ja ollaan seeklevelillä alimman arvon alapuolella, niin palautetaan alin indeksi
                    }
                }
                else
                { // Etsitään indeksi
                    if (slotlistcount == 1)
                    {
                        if (aboveindex == false)
                        {
                            retVal = 0; // Selkeä tilanne, jos kohteita on vain 1
                        }
                        else
                        {
                            if (seeklevel == this.invindexlist.ElementAt(0).Key)
                            {
                                retVal = 0; // Jos ainoa indeksi on sama kuin mitä etsitään, niin palautetaan se
                            }
                            else
                            {
                                retVal = -10; // Jos aboveindex=true ja seeklevel on suurempi kuin ainoa indeksi, niin palautetaan -10;
                            }
                        }
                    }
                    else
                    {
                        if (seeklevel >= this.invindexlist.ElementAt(slotlistcount - 1).Key)
                        { // Jos etsittävä listan suurimman arvon yläpuolella
                            if (aboveindex == false)
                            {
                                retVal = slotlistcount - 1; // Selkeä tilanne, jos seeklevel on suurimman arvon yläpuolella tai sama arvo ja aboveindex=false
                            }
                            else
                            {
                                if (seeklevel == this.invindexlist.ElementAt(slotlistcount - 1).Key)
                                {
                                    retVal = slotlistcount - 1;
                                }
                                else
                                {
                                    retVal = -10; // Palauttaa -10 arvon, jos aboveindex=true ja arvo on yli suurimman arvon listassa
                                }
                            }
                        }
                        else
                        {
                            minval = 0;
                            mid = slotlistcount / 2;
                            maxval = slotlistcount - 1;
                            while (true)
                            {
                                if (maxval - minval < amo)
                                { // Jos indeksien erotus on jo pieni, niin käydään loput läpi for loopilla
                                    if (minval + amo > maxval)
                                    { // Määritellään minimi ja maksimi indeksien arvot haettavalle alueelle
                                        tarval = maxval;
                                    }
                                    else
                                    {
                                        tarval = minval + amo;
                                    }
                                    if (seeklevel == this.invindexlist.ElementAt(minval).Key)
                                    { // Tarkastetaan, ettei arvo ole haettavan kokonaisuuden alarajalla
                                        retVal = minval;
                                        break;
                                    }
                                    else
                                    {
                                        for (i = minval; i < tarval; i++)
                                        { // Käydään indeksit läpi minimirajalta maksimirajalle -1, koska sitten if lauseessa käytetään i+1 indeksiä
                                            if (seeklevel > this.invindexlist.ElementAt(i).Key && seeklevel <= this.invindexlist.ElementAt(i + 1).Key)
                                            { // Jos löytyy aiotusta välistä
                                                retVal = i + 1;
                                                break;
                                            }
                                        }
                                        if (retVal >= 0)
                                        {
                                            break; // Poistutaan while loopista, jos oikea kohde löytyi for loopin aikana
                                        }
                                    }
                                }
                                else
                                { // Jos indeksien osalta on vielä runsaasti matkaa toisiinsa, niin jatketaan puolitusmenetelmää
                                    if (seeklevel > this.invindexlist.ElementAt(mid).Key)
                                    {
                                        minval = mid; // minval muuttuu, maxval pysyy samana
                                    }
                                    else
                                    {
                                        maxval = mid; // maxval muuttuu, minval pysyy samana
                                    }
                                    apu = (maxval - minval) / 2;
                                    mid = apu + minval; // Luodaan uusi mid arvo puolitusmenetelmää varten
                                }
                            }

                            apu = retVal;
                            // Tähän saakka haettu indeksi arvo on oikein, jos on haettu oletuksella aboveindex=true. Nyt muokataan indeksin arvoa, jos aboveindex=false
                            if (aboveindex == false)
                            {
                                if (seeklevel == this.invindexlist.ElementAt(0).Key)
                                {
                                    retVal = 0;
                                }
                                else if (seeklevel == this.invindexlist.ElementAt(apu).Key)
                                {
                                    retVal = apu;
                                }
                                else
                                {
                                    retVal = apu - 1;
                                }
                            }
                        }
                    }
                }
            }

            return retVal;
        }
       
    }

    /// <summary> Tämä luokka voidaan lisätä indeksiksi eri kohteille. Luokka pitää kohteet järjesteyksessä, josta ne on nopea hakea. Luokka tekee käänteisen indeksin, jossa slotvalue on key ja unique number osa indeksoitua objektia. Tämä on kyseisestä luokasta decimal - UID indeksi 
    /// HUOM! Tätä Long muotoista indeksointia ei ole testattu vielä kaikilta osin
    /// </summary>
    public class OrderIndexInt : OrderIndex<int>
    {
        private int registeredtype;

        /// <summary> Referenssi käyttöliittymään </summary>
        private ProgramHMI prhmi; 
        
        /// <summary> Inverted indeksi, eli ensin on kurssin arvo (Key) ja sitten OneIndex luokka sisältää areapointerin sekä uniikinid numeron </summary> 
        //private SortedList<decimal, OneIndex> invindexlist;

        //private OneIndex ehkarefoneindex;

        /// <summary>
        /// Constructor - tämä luokka voidaan alustaa erillisen indeksin, jolle voidaan lisätä alkuperäisen aspektin lisäksi muita aspekteja
        /// </summary>
        /// <param name="prograhmi"> ProgramHMI, käyttöliittymän referenssi </param>
        /// <returns> {void} </returns>
        public OrderIndexInt(ProgramHMI prograhmi) : base(prograhmi) { 
            this.prhmi = prograhmi;
            this.registeredType = (int)OrderIndexType.INDEX_TYPE_INT_1;
        }

        /// <summary> Palauttaa alimman kohteen key valuen </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {long}, palauttaa -1 jos virhe, muussa tapauksessa pienimmän kohteen key arvon </returns> 
        public override int LowestKeyValue(string kutsuja, bool suppresserror=false)
        {
            string funimi="->(OII)LowestKeyValue";
            int retVal=-1;

            if (this.Amount(kutsuja+funimi)>0) {
                retVal=this.invindexlist.ElementAt(0).Key;
            } else {
                if (suppresserror==false) this.prhmi.sendError(kutsuja+funimi,"No key->value pairs in invindexlist!",-989,2,4);
            }
            return retVal;
        }

        /// <summary> Palauttaa suurimman kohteen key valuen - TODO: epäily, että tämä käsky ei kertoisikaan suurinta arvoa listassa! </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="suppresserror"> bool, kirjoitetaanko virhe, jos ei ollut yhtään kohdetta listassa </param>
        /// <returns> {decimal}, palauttaa -1 jos virhe, muussa tapauksessa suurimman kohteen key arvon </returns>        
        public override int HighestKeyValue(string kutsuja, bool suppresserror=false)
        {
            string funimi="->(OII)HighestKeyValue";
            int retVal=-1;
            int amo=-1;

            if (this.Amount(kutsuja+funimi)>0) {
                amo=this.Amount(kutsuja+funimi);
                retVal=this.invindexlist.ElementAt(amo-1).Key;
            } else {
                if (suppresserror==false) this.prhmi.sendError(kutsuja+funimi,"No key->value pairs in invindexlist!",-1001,2,4);
            }
            return retVal;
        }

        /// <summary> Tekee saman, kuin IndexOfKey suoraan </summary>
        public override int IndexOfKey(string kutsuja, int seeklevel)
        {
            return this.invindexlist.IndexOfKey(seeklevel);
        }

        /// <summary> Tämä funktio lisää sortedlist listaan slotin int arvon keyksi ja valueksi se tallentaa OneIndex objektin, johon on tallennettu yksilöllinen uniquenumber sekä areapointerin arvo </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="slotvalue"> int, slotin arvo, jonka alueella myynti tapahtui </param>
        /// <param name="uniqnumb"> long, kohdeslotin uniikki referenssi numero </param>
        /// <param name="areap"> int, mihinkä kohteeseen/listaan kyseinen operaatioarvo liittyy </param>
        /// <param name="usenegativeslotvalue"> bool, voidaanko käyttää negatiivisia arvoja slotvalue:n arvona vai ei </param>
        /// <param name="usenegativeUID"> bool, voidaanko käyttää negatiivisia arvoja UID arvoina (uniqnub) vai ei </param>
        /// <returns> {int} jos pienempi kuin 0, niin kyseessä virhekoodi ja koodi kertoo, minkälainen virhe oli kyseessä. Jos=0, niin lisäsi arvot, mutta listassa oli jo sillä slotin arvolla toinen arvo, joka oli identtinen. Jos=1, niin lisäsi kohteen listaan. Jos=2, niin samalla arvolla oli toinen kohde, mutta ei identtinen, joten lisäsi kohteen listaan </returns>
        public override int AddValuesToIndexes(string kutsuja, int slotvalue, long uniqnumb, int areap, bool usenegativeslotvalue=false, bool usenegativeUID=false)
        {
            string funimi="->(OII)addValuesToIndexes";
            int retVal=-1;
            int amo=this.invindexlist.Count;
            int indofk=-1;
            bool enterfurtherslotval=false;
            bool enterfurtheruid=false;

            
            if (slotvalue<0 && usenegativeslotvalue==false) {
                enterfurtherslotval=false;
            } else {
                enterfurtherslotval=true;
            }

            if (enterfurtherslotval==true) {
                
                if (uniqnumb<0 && usenegativeUID==false) {
                    enterfurtheruid=false;
                } else {
                    enterfurtheruid=true;
                }

                if (enterfurtheruid==true) {
                    indofk=this.invindexlist.IndexOfKey(slotvalue);
                    if (indofk>=0) { // Kohde on jo listassa
                        if (this.invindexlist.ElementAt(indofk).Value.AreaPointer==areap && this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber==uniqnumb) {
                            this.invindexlist.ElementAt(indofk).Value.AreaPointer=areap;
                            this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber=uniqnumb;

                            this.prhmi.sendError(kutsuja+funimi,"There was already a identical slot in asset list! Replacing values! Slotval:"+slotvalue+" Uniqnum:"+uniqnumb+" Areap:"+areap,-994,2,4);
                            retVal=0;
                        } else {
                            if (this.invindexlist.ElementAt(indofk).Value.AreaPointer==areap) {
                                this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber=uniqnumb;
                                retVal=2;
                            } else {
                                this.prhmi.sendError(kutsuja+funimi,"There was already a slot in asset list! Adding new copy! Slotval:"+slotvalue+" Uniqnum:"+uniqnumb+" Areap:"+areap+" OldUniqnum:"+this.invindexlist.ElementAt(indofk).Value.UniqueRefNumber+" OldAreap:"+this.invindexlist.ElementAt(indofk).Value.AreaPointer,-995,4,4);
                                this.invindexlist.Add(slotvalue,new OneIndex(areap,uniqnumb)); // Lisätään uusi indeksi, jos ei ollut ennestään 
                                retVal=-4;
                            }
                        }
                    } else { // Kohde ei ole listassa, joten lisätään uusi listaan
                        this.invindexlist.Add(slotvalue,new OneIndex(areap,uniqnumb)); // Lisätään uusi indeksi, jos ei ollut ennestään 
                        retVal=1;
                    }
                } else {
                    retVal=-3; // Yksilöllinen numero (UID) ei ollut sallituissa rajoissa
                }
            } else retVal=-2; // Slotin numero ei ollut sallituissa rajoissa
        
            return retVal;
        }

        /// <summary> Tämä funktio poistaa uniquenum numeroisen kohteen käänteisen indeksin listalta, jota se etsii slotvalue paikasta </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="uniquenum"> long, kohdeslotin uniikki referenssi numero </param>
        /// <param name="slotvalue"> int, slotin arvo, jota ollaan poistamassa käänteisistä indekseistä </param>
        /// <returns> {int} 2=jos poisto onnistui, 1=jos lista oli tyhjä, 0=jos kohdetta ei ollut listalla, pienempi kuin 0, jos virhe </returns>
        public override int RemoveValuesFromIndexes(string kutsuja, long uniquenum, int slotvalue)
        {
            int retVal=-1;
            bool succrem=false;
            string funimi="->(OII)removeValuesFromIndexes";
            int indexf=-1;
            int iok=-1;

            if (this.invindexlist.Count>0) {
                iok=this.invindexlist.IndexOfKey(slotvalue);
                if (iok>=0) { // Kohde on jo listassa
                    if (this.invindexlist.ElementAt(iok).Value.UniqueRefNumber==uniquenum) { // Tarkistetaan, onko sisässä asetettuna uniikki numero vai ei
                        succrem=this.invindexlist.Remove(slotvalue);
                        if (succrem==false) {
                            retVal=-3;
                            this.prhmi.sendError(kutsuja+funimi,"Unexpected index list removing error! Slotval:"+slotvalue+" Uniqnum:"+uniquenum,-993);
                        } else {
                            retVal=2;
                            invindexlist.TrimExcess();
                        }
                        return retVal;
                    } 
                }

                // Jos ei löytynyt tai uniquenum ei täsmännyt, niin..
                indexf=this.SeekIndexByUniqueId(kutsuja+funimi,uniquenum); // Etsitään kohdetta for loopilla

                if (indexf>=0) { // Kohde löytyi 
                    this.invindexlist.RemoveAt(indexf); // Poistetaan kohde
                    retVal=2;
                } else {
                    if (indexf==-20) { retVal=1; }
                    if (indexf==-10) { retVal=0; }
                }
            } else retVal=1; 

            return retVal;
        }     

        /// <summary> Tämä funktio palauttaa kohteen key arvon (kurssiarvo), silloin kun kohteen indeksi listassa on annettu </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="invlistindex"> int, käänteisen indekserin indeksi, jota kohdetta etsitään </param>        
        public override int ReturnKeyByIndex(string kutsuja, int invlistindex)
        {
            int retVal=-1;
            int amo=this.invindexlist.Count;

            if (amo>0) { // Onko kohteita listassa enemmän kuin 0
                if (invlistindex<amo) { // Onko indeksi listan alueella
                    retVal=this.invindexlist.ElementAt(invlistindex).Key;
                } else retVal=-3;
            } else retVal=-2;

            return retVal;
        }

        /// <summary> Tämä funktio palauttaa uniqueidnumberin käänteisestä indeksilistasta, jossa seekval on sen slotin arvo, jota etsitään </summary>
        /// <param name="kutsuja"> string, kutsujan polku, joka kutsuu tätä kyseistä funktiota </param>
        /// <param name="seekval"> long, nykykurssin hinta tai muu arvo, jota vastaavaa slottiarvoa etsitään </param>
        /// <param name="seektype"> int, 0=etsitään nimenomaan sitä arvoa, joka on seekval muuttujassa, 1=etsitään sitä arvoa joka on seekval muuttujassa tai pienempää arvoa, mutta kuitenkin suurempaa kuin seuraavaksi pienin arvo, 2=etsitään sitä arvoa, joka on seekval muuttujasa, mutta kuitenkin suurempi kelpuutetaan, jos silti pienempi kuin seuraavaksi suurin arvo </param>
        /// <param name="useforcedareap"> int, pakotetaan löydetystä kohteesta tarkistamaan, onko areapointer myös sama, kuin parametriin annettu ennen kohteen palauttamista käyttäjälle. Jos pienempi kuin 0, niin ei käytetä tätä tarkistusta </param>
        /// <returns> {int}, jos suurempi tai yhtäsuuri kuin 0 palauttaa löydetyn kohteen uniqueidnum arvon, -1=kohde ei löytynut, -2 ja -3 = kohde löytyi, mutta ei ollut sama useforcedareap parametri, joten palautettiin negatiivinen luku, -4 = listan pituus 0, 
        /// -6 = jotain hämärää, kun kohde meni listan läpi, eikä reagoinut, jos seektype=1 ja seekval pienempi kuin alin slotin arvo, niin palauttaa -21 ja jos seektype=2 ja seekval suurempi kuin ylin slotin arvo, niin palauttaa -22
        /// </returns>
        public override long ReturnUniqueIdNum(string kutsuja, int seekval, int seektype=0, int useforcedareap=-1)
        {
            string funimi="->(OII)returnUniqueIdNum";
            long retVal=-6;
            int amo=invindexlist.Count;
            bool aboveindx;
            int retind=-1;
            
            if (amo>0) {
                // Etsitään pelkästään kohdearvoa ja palautetaan -1, mikäli kohdetta ei löydy
                if (this.invindexlist.IndexOfKey(seekval)>-1) {
                    if (useforcedareap<0) { // Kohde löytyi, eikä tarvitse tehdä areapointer tarkistusta
                        retVal=this.invindexlist[seekval].UniqueRefNumber;
                        return retVal;
                    } else {
                        if (this.invindexlist[seekval].AreaPointer==useforcedareap) {
                            retVal=this.invindexlist[seekval].UniqueRefNumber;
                            return retVal;                                
                        } else {
                            retVal=-2; // Kohde löytyi, mutta sen areapointer ei ollut sama, kuin useforcedareap parametrissa haettu arvo
                            return retVal; // TODO: HUOM, halutaanko että etsintä keskeytyy tähän, vai halutaanko että etsitään lähistöltä muita vastaavia kohteita, joissa areapointer voisi täsmätä?
                        }
                    }
                }
                if (seektype>0) { // Etsitään kohdearvoa tai sitä suurempaa / pienempää arvoa riippuen, mitä parametriin oli laitettu
                    retVal=-1;
                    if (seektype==1) aboveindx=false; else aboveindx=true; // Tutkitaan, minkälainen hakutapa meillä on kohteelle

                    retind=this.FindInvertedIndex(kutsuja+funimi,seekval,aboveindx); // Etsitään, löytyykö haluttua kohdetta indeksilistasta
                    if (retind>=0) {
                        //this.ehkarefoneindex=this.invindexlist.ElementAt(retind).Value; // Jos löytyy, niin otetaan objekti referenssi talteen
                        if (useforcedareap<0) { // Tehdäänkö pakkotarkistus, oliko areapointer sama - jos <0, niin ei pakkotarkisteta
                            retVal=this.invindexlist.ElementAt(retind).Value.UniqueRefNumber;
                            return retVal;
                        } else { // Tässä tapauksessa tehdään areapointerin suhteen pakkotarkistus
                            if (this.invindexlist.ElementAt(retind).Value.AreaPointer==useforcedareap) {
                                retVal=this.invindexlist.ElementAt(retind).Value.UniqueRefNumber;
                                return retVal;                                
                            } else {
                                retVal=-3; // Kohde löytyi, mutta sen areapointer ei ollut sama, kuin useforcedareap parametrissa haettu arvo
                                return retVal; // TODO: HUOM, halutaanko että etsintä keskeytyy tähän, vai halutaanko että etsitään lähistöltä muita vastaavia kohteita, joissa areapointer voisi täsmätä?
                            }
                        }
                    } else {
                        if (retind==-1 || retind==-10) { // Jos aboveindex=false ja seekval<alin slotin arvo, niin palauttaa -1 ja jos aboveindex=true ja seekval>ylin slotin arvo, niin palauttaa -10
                            if (retind==-1) {
                                retVal=retind-20; // Vanha -1, nyt palauttaa -21
                            } else {
                                retVal=retind-12; // Vanha -10, nyt palauttaa -22
                            }
                            this.prhmi.sendError(kutsuja+funimi,"Seeked value out of list values! Seektype:"+seektype+" Seeklevel:"+seekval+" Retind:"+retind+" RetVal:"+retVal,-991,2,4);
                        } else {
                            retVal=-5;
                            this.prhmi.sendError(kutsuja+funimi,"Error on seeking right index value! Error after seeking:"+retind+" Slotval:"+seekval+" Type:"+seektype+" Areap:"+useforcedareap,-992,4,4);
                        }
                    }
                }
            } else {
                retVal=-4;
            }

            return retVal;
        }

        /// <summary>
        /// Tämä funktio etsii slottia vastaavan indeksoidun luvun, joka on juuri nykykurssin hinnan yläpuolella tai sama kuin nykykurssi.
        /// </summary>
        /// <param name="kutsuja">string, kutsujan polku, joka kutsuu tätä kyseistä funktiota</param>
        /// <param name="seeklevel">int, nykykurssin hinta, jota vastaavaa slottiarvoa etsitään</param>
        /// <param name="aboveindex">bool, jos true, niin palauttaa sen indeksin, joka on kyseistä kurssia suurempi seuraava slotin indeksi. Jos false, niin tällöin pienempi indeksi</param>
        /// <returns>int palauttaa sen indeksin tämän luokan käännetyn indeksin listasta, jossa slottiarvo on sama tai pienempi, kuin seeklevel. Jos nykykurssi pienempi kuin alin slottiarvo, niin palauttaa -1, jos käännetyssä indeksilistassa ei ole yhtään slottiarvon vastinetta, palauttaa -4, jos aboveindex=true ja seeklevel yli suurimman arvon, niin palauttaa -10</returns>
        public override int FindInvertedIndex(string kutsuja, int seeklevel, bool aboveindex = true)
        {
            int retVal = -3;
            int slotlistcount = -1;
            int mid = -1;
            int apu = -1;
            int minval = -1;
            int maxval = -1;
            int i = 0;
            int amo = 5;
            int tarval = 0;

            slotlistcount = this.invindexlist.Count;

            if (slotlistcount == 0)
            { // Jos kohteita on nolla
                retVal = -4;
            }
            else
            {
                if (seeklevel < this.invindexlist.ElementAt(0).Key)
                { // Jos etsittävä listan pienimmän arvon alapuolella
                    if (aboveindex == false)
                    {
                        retVal = -1; // Jos aboveindex=false ja ollaan alle alimman arvon, niin palautetaan -1
                    }
                    else
                    {
                        retVal = 0; // Jos aboveindex=true ja ollaan seeklevelillä alimman arvon alapuolella, niin palautetaan alin indeksi
                    }
                }
                else
                { // Etsitään indeksi
                    if (slotlistcount == 1)
                    {
                        if (aboveindex == false)
                        {
                            retVal = 0; // Selkeä tilanne, jos kohteita on vain 1
                        }
                        else
                        {
                            if (seeklevel == this.invindexlist.ElementAt(0).Key)
                            {
                                retVal = 0; // Jos ainoa indeksi on sama kuin mitä etsitään, niin palautetaan se
                            }
                            else
                            {
                                retVal = -10; // Jos aboveindex=true ja seeklevel on suurempi kuin ainoa indeksi, niin palautetaan -10;
                            }
                        }
                    }
                    else
                    {
                        if (seeklevel >= this.invindexlist.ElementAt(slotlistcount - 1).Key)
                        { // Jos etsittävä listan suurimman arvon yläpuolella
                            if (aboveindex == false)
                            {
                                retVal = slotlistcount - 1; // Selkeä tilanne, jos seeklevel on suurimman arvon yläpuolella tai sama arvo ja aboveindex=false
                            }
                            else
                            {
                                if (seeklevel == this.invindexlist.ElementAt(slotlistcount - 1).Key)
                                {
                                    retVal = slotlistcount - 1;
                                }
                                else
                                {
                                    retVal = -10; // Palauttaa -10 arvon, jos aboveindex=true ja arvo on yli suurimman arvon listassa
                                }
                            }
                        }
                        else
                        {
                            minval = 0;
                            mid = slotlistcount / 2;
                            maxval = slotlistcount - 1;
                            while (true)
                            {
                                if (maxval - minval < amo)
                                { // Jos indeksien erotus on jo pieni, niin käydään loput läpi for loopilla
                                    if (minval + amo > maxval)
                                    { // Määritellään minimi ja maksimi indeksien arvot haettavalle alueelle
                                        tarval = maxval;
                                    }
                                    else
                                    {
                                        tarval = minval + amo;
                                    }
                                    if (seeklevel == this.invindexlist.ElementAt(minval).Key)
                                    { // Tarkastetaan, ettei arvo ole haettavan kokonaisuuden alarajalla
                                        retVal = minval;
                                        break;
                                    }
                                    else
                                    {
                                        for (i = minval; i < tarval; i++)
                                        { // Käydään indeksit läpi minimirajalta maksimirajalle -1, koska sitten if lauseessa käytetään i+1 indeksiä
                                            if (seeklevel > this.invindexlist.ElementAt(i).Key && seeklevel <= this.invindexlist.ElementAt(i + 1).Key)
                                            { // Jos löytyy aiotusta välistä
                                                retVal = i + 1;
                                                break;
                                            }
                                        }
                                        if (retVal >= 0)
                                        {
                                            break; // Poistutaan while loopista, jos oikea kohde löytyi for loopin aikana
                                        }
                                    }
                                }
                                else
                                { // Jos indeksien osalta on vielä runsaasti matkaa toisiinsa, niin jatketaan puolitusmenetelmää
                                    if (seeklevel > this.invindexlist.ElementAt(mid).Key)
                                    {
                                        minval = mid; // minval muuttuu, maxval pysyy samana
                                    }
                                    else
                                    {
                                        maxval = mid; // maxval muuttuu, minval pysyy samana
                                    }
                                    apu = (maxval - minval) / 2;
                                    mid = apu + minval; // Luodaan uusi mid arvo puolitusmenetelmää varten
                                }
                            }

                            apu = retVal;
                            // Tähän saakka haettu indeksi arvo on oikein, jos on haettu oletuksella aboveindex=true. Nyt muokataan indeksin arvoa, jos aboveindex=false
                            if (aboveindex == false)
                            {
                                if (seeklevel == this.invindexlist.ElementAt(0).Key)
                                {
                                    retVal = 0;
                                }
                                else if (seeklevel == this.invindexlist.ElementAt(apu).Key)
                                {
                                    retVal = apu;
                                }
                                else
                                {
                                    retVal = apu - 1;
                                }
                            }
                        }
                    }
                }
            }

            return retVal;
        }
       
    }

    /// <summary> Tämä luokka pitää sisällään vain yhden areapointer arvon sekä uniikin id:n itse kohteeseen. </summary>
    public class OneIndex
    {
        /// <summary>
        /// Jos kohde voi olla useassa jossain ali-indeksissä kaikkien kohteiden joukosta, sen joukon indeksi johon kohde kuuluu. Esim. jos meillä on "MyOwnClass[] arrayOfMyClassObjects", niin areapointerin ollessa 2, tarkoittaisi että tällöin kohde löytyisi arrayOfMyClassObjects[2] listan indeksoiduista listoista. Jos -1, jos ei kuulu mihinkään ali-indeksiin.
        /// </summary>
        public int AreaPointer { get; set; }

        /// <summary>
        /// Kohteen yksilöllinen UID
        /// </summary>
        public long UniqueRefNumber { get; set; }

        /// <summary>
        /// Constructor - luo kohteen arepointer sekä UID tietojen pohjalta
        /// </summary>
        /// <param name="areap">int, Jos kohde voi olla useassa jossain ali-indeksissä kaikkien kohteiden joukosta, sen joukon indeksi johon kohde kuuluu. Esim. jos meillä on "MyOwnClass[] arrayOfMyClassObjects", niin areapointerin ollessa 2, tarkoittaisi että tällöin kohde löytyisi arrayOfMyClassObjects[2] listan indeksoiduista listoista. Jos -1, jos ei kuulu mihinkään ali-indeksiin.</param>
        /// <param name="uniqueref">long, Kohteen yksilöllinen UID</param>
        /// <returns> {void} </returns>
        public OneIndex(int areap, long uniqueref)
        {
            this.AreaPointer=areap;
            this.UniqueRefNumber=uniqueref;
        }
    }