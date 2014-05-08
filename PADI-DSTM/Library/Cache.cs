using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary {
    class ClientCache {

        /// <summary>
        /// Structure that maps UID to PadInt's value
        /// </summary>
        private Dictionary<int, int> padIntsCache;
        /// <summary>
        /// Structure that stores uid of writes and server's address
        /// </summary>
        private Dictionary<int, string> cacheWrites;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverAddress">Server Address</param>
        internal ClientCache() {
            padIntsCache = new Dictionary<int, int>();
            cacheWrites = new Dictionary<int, string>();
        }

        /// <summary>
        /// PadInt cache accessor
        /// </summary>
        internal Dictionary<int, int> Cache {
            get { return padIntsCache; }
        }

        /// <summary>
        /// PadInt list accessor
        /// </summary>
        internal Dictionary<int, string> CacheWrites {
            get { return cacheWrites; }
        }

        /// <summary>
        /// See if a previous write has occurred
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <returns>True if a previous write has occurred</returns>
        internal bool hasPreviousWrite(int uid) {
            return cacheWrites.ContainsKey(uid);
        }

        /// <summary>
        /// See if a given uid is in cache
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <returns>Returns true if the uid is in cache</returns>
        internal bool hasUidInCache(int uid) {
            return padIntsCache.ContainsKey(uid);
        }

        /// <summary>
        /// Obtains PadInt's value stored in cache
        /// </summary>
        /// <param name="uid">PadInt's value</param>
        /// <returns>PadInt's value</returns>
        internal int getValueInCache(int uid) {
            return padIntsCache[uid];
        }

        /// <summary>
        /// Updates PadInt's read value store in cache
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <param name="value">Value to assign</param>
        internal void updateReadValue(int uid, int value) {
            if(padIntsCache.ContainsKey(uid)) {
                padIntsCache[uid] = value;
            } else {
                padIntsCache.Add(uid, value);
            }
        }

        /// <summary>
        /// Updates PadInt's write value store in cache
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <param name="value">Value to assign</param>
        internal void updateWriteValue(int uid, int value) {

            padIntsCache[uid] = value;
        }

        /// <summary>
        /// Updates PadInt's write value store in cache and adds the uid to the cacheWrites
        /// </summary>
        /// <param name="uid">PadInt's uid</param>
        /// <param name="value">Value to assign</param>
        internal void addWriteValue(int uid, int value, string address) {
            if(!cacheWrites.ContainsKey(uid)) {
                cacheWrites.Add(uid, address);
            }

            if(padIntsCache.ContainsKey(uid)) {
                padIntsCache[uid] = value;
            } else {
                padIntsCache.Add(uid, value);
            }
        }
    }
}
