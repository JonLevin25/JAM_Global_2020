using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace Character.Scripts
{
   [RequireComponent(typeof(BoxCollider2D))]
   public class LadderScript : MonoBehaviour
   {
      public List<Collider2D> groundCollider2Ds;
   }
   
   #if UNITY_EDITOR
   class LadderCustomBuildProcessor : IProcessSceneWithReport
   {
      public int callbackOrder { get { return 0; } }
      public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
      {
         Debug.Log("LadderCustomBuildProcessor.OnProcessScene " + scene.name);

         var ladders = GameObject.FindObjectsOfType<LadderScript>();
         foreach (var ladder in ladders)
         {
            BoxCollider2D boxCollider = ladder.GetComponent<BoxCollider2D>();
            var transform = ladder.transform;
            Vector2 origin = (Vector2) transform.position + boxCollider.offset;
            Vector2 size = boxCollider.size * (Vector2)transform.lossyScale;
            RaycastHit2D[] results = Physics2D.BoxCastAll(origin, size, 0, Vector2.up, 0);

            if (ladder.groundCollider2Ds == null)
            {
               ladder.groundCollider2Ds = new List<Collider2D>();
            }

            var ladderColliders = ladder.GetComponents<Collider2D>();
            foreach (var result in results)
            {
               Collider2D collider = result.collider;
               if (ladderColliders.Contains(collider))
               {
                  continue;
               }
               ladder.groundCollider2Ds.Add(collider);
            }
         }
      }
   }
   #endif
}
