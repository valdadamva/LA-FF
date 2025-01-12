using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
   public float moveSpeed = 5f;  // скорость движения игрока
   public float rotationSpeed = 700f; // скорость поворота

   private Rigidbody rb;

   void Awake()
   {
      // Получаем компонент Rigidbody
      rb = GetComponent<Rigidbody>();
   }

   void Update()
   {
      // Если это не локальный игрок, не выполняем управление
      if (!isLocalPlayer)
         return;

      MovePlayer();
   }

   void MovePlayer()
   {
      // Получаем значения для движения по осям X и Z (вперед/назад и влево/вправ)
      float horizontal = Input.GetAxis("Horizontal"); // A/D или стрелки влево/вправ
      float vertical = Input.GetAxis("Vertical"); // W/S или стрелки вверх/вниз

      // Двигаем игрока вперед/назад и влево/вправo
      Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized * moveSpeed * Time.deltaTime;
      rb.MovePosition(transform.position + movement);

      // Поворот игрока в направлении движения
      if (movement.magnitude > 0f)
      {
         Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
         transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
      }
   }
}

