using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
   [SerializeField] private GameObject _leftwall;
   [SerializeField] private GameObject _rightwall;
   [SerializeField] private GameObject _frontwall;
   [SerializeField] private GameObject _backwall;
   [SerializeField] private GameObject _unvisitedBlock;
  

   public bool isVisited { get; private set; }

   public void Visit()
   {
       isVisited = true;
       _unvisitedBlock.SetActive(false);
   }
   public void Clearleft()
   {
       _leftwall.SetActive(false);
   }
   public void Clearright()
   {
       _rightwall.SetActive(false);
   }
   public void Clearfront()
   {
       _frontwall.SetActive(false);
   }
   public void Clearback()
   {
       _backwall.SetActive(false);
   }

   public Vector3 GroundRaycast()
   {
       RaycastHit hit;
       if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, Mathf.Infinity))
       {
           
       }
       return new Vector3(0,hit.distance,0);
   }



}
