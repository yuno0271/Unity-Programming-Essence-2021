﻿using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour 
{
   public AudioClip deathClip; // 사망시 재생할 오디오 클립
   public float jumpForce = 700f; // 점프 힘

   private int jumpCount = 0; // 누적 점프 횟수
   private bool isGrounded = false; // 바닥에 닿았는지 나타냄
   private bool isDead = false; // 사망 상태

   private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
   private Animator animator; // 사용할 애니메이터 컴포넌트
   private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

   private void Start() 
   {
      // 초기화
      playerRigidbody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      playerAudio = GetComponent<AudioSource>();
   }

   private void Update()
   {
        if (isDead) return;

        if(Input.GetMouseButtonDown(0) && jumpCount < 2)
        {
            // 점프 횟수 증가
            jumpCount++;
            // 점프 직전에 속도를 순간적으로 제로(0,0)로 변경
            playerRigidbody.velocity = Vector2.zero;
            // 리지드바디에 위쪽으로 힘 주기
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            // 오디오 소스 재생
            playerAudio.Play();
        }
        else if(Input.GetMouseButtonUp(0) && playerRigidbody.velocity.y > 0)
        {
            // 마우스 왼쪽 버튼에서 손을 떼는 순간 && 속도의 y값이 양수라면(위로 상승 중)
            // 현재 속도를 절반으로 변경
            playerRigidbody.velocity = playerRigidbody.velocity * 0.5f;
        }

        // 애니메이터의 Ground 파라미터를 isGrounded 값으로 갱신
        animator.SetBool("Grounded", isGrounded);
   }

   private void Die() 
   {
        // 사망 처리
        animator.SetTrigger("Die");
        // 사망 효과음 재생
        playerAudio.clip = deathClip;
        playerAudio.Play();

        // 속도를 제로(0,0)로 변경
        playerRigidbody.velocity = Vector2.zero;
        isDead = true;

        // 게임 매니저의 게임오버 처리 실행
        GameManager.instance.OnPlayerDead();
   }

   private void OnTriggerEnter2D(Collider2D other) 
   {
       // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
       if(other.tag == "Dead" && !isDead)
        {
            Die();
        }
   }

   private void OnCollisionEnter2D(Collision2D collision) 
   {
        // 어떤 콜라이더와 닿았으며, 충돌 표면이 위쪽을 보고 있으면,
        if (collision.contacts[0].normal.y > 0.7f)
        {
            isGrounded = true;
            jumpCount = 0;
        }
   }

   private void OnCollisionExit2D(Collision2D collision)
   {
        // 바닥에서 벗어났음을 감지하는 처리
        // 어떤 콜라이더에서 떼어진 경우, isGrounded를 false로 변경
        isGrounded = false;
   }
}