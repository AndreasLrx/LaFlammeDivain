using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   Rigidbody2D body;

   float horizontal;
   float vertical;
   float moveLimiter = 0.7f;

   public float runSpeed = 10.0f;
   public GameObject wispsGroupPrefab;

   void Start()
   {
      body = GetComponent<Rigidbody2D>();
      Instantiate(wispsGroupPrefab, transform);
   }

   void Update()
   {
      // Gives a value between -1 and 1
      horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
      vertical = Input.GetAxisRaw("Vertical"); // -1 is down

      if (Input.GetButtonDown("ActivateWisp"))
      {
         Wisp wisp = GetWisps().GetSelectedWisp();
         if (wisp != null)
            StartCoroutine(wisp.Activate());
      }

   }

   WispsGroup GetWisps()
   {
      return gameObject.GetComponentInChildren<WispsGroup>();
   }

   void FixedUpdate()
   {
      if (horizontal != 0 && vertical != 0) // Check for diagonal movement
      {
         // limit movement speed diagonally, so you move at 70% speed
         horizontal *= moveLimiter;
         vertical *= moveLimiter;
      }

      body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
   }

   public Vector2 AimedDirection()
   {
      return ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
   }

   public void AddWisp(GameObject wispObject)
   {
      wispObject.GetComponent<Wisp>().playerObject = gameObject;
      GetWisps().AddWisp(wispObject);
   }
}