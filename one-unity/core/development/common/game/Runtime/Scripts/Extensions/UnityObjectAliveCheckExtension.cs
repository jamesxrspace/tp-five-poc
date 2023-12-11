namespace TPFive.Game
{
    public static class UnityObjectAliveCheckExtension
    {
        /// <summary>
        /// Check the object is still alive.<br/><br/>
        /// Help the case of Unity Object not support the null-conditional operator,
        /// so we can use this extension method to check the object is still alive,
        /// no matter the object is Unity Object or not.
        /// </summary>
        /// <param name="obj">The object want to check whether still alive.</param>
        /// <returns>If TRUE means is alive, otherwise not.</returns>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/Object.html"/>
        public static bool IsAlive(this IHasAliveCheck obj)
        {
            if (obj is UnityEngine.Object o)
            {
                return o != null;
            }

            return obj != null;
        }
    }
}
