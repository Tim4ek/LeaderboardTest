using UnityEngine;

namespace com.Common.Extensions {
  public static class UnityObjectExtension {
		public static void UnityDestroy(this Object obj) {
			UnityEngine.Object.Destroy(obj);
		}
	}
}