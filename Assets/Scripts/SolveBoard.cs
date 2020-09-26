﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolveBoard : MonoBehaviour
{
    public string buttontag;
    public GameConfig gameConfig;
    private bool startMatching;
    private int blockCount;
    private Delay delay;
    private float interval;
    private SpriteRenderer render;
    public Sprite btnPressedSprite;

    private void Start()
    {
        blockCount = GameObject.FindGameObjectsWithTag("gameblock").Length;
        startMatching = false;
        delay = new Delay(1.0f);
        interval = 0F;
        render = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(pos));
            if (hitCollider != null && hitCollider.CompareTag(buttontag))
            {
                render.sprite = btnPressedSprite;
                startMatching = true;
                delay.Reset();
            }
        }

        if (startMatching)
        {
            if (delay.IsReady)
            {
                MatchAllBlocks();
                delay.Reset();

                // This is a really crappy way of ending the matching cycle
                // The first time it checks for blocks moving, it's too fast so it doesnt register the block moving
                // and this ends the cycle. in order to prevent that, I'm using a variable to check if moving blocks returned 0 twice (no blocks moving)
                // if it does, that means the check is accurate so the board has reached a static state, meaning there are no more matches to be made
                if (CheckStillBlocks() == 0F) //No blocks are moving
                {
                    interval += 1F;
                }
                else
                {
                    interval = 0F;
                }

                if (interval == 2F) //If no blocks were moving during 2 checks in a row
                {
                    startMatching = false; //Stop the matching loop
                    //Debug.Log("Matches ended!");
                }
            }
        }
    }

    private void MatchAllBlocks()
    {
        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("gameblock"))
        {
            if (fooObj.name == "Block(Clone)")
            {
                BlockClass solver = (BlockClass)fooObj.GetComponent(typeof(BlockClass));
                solver.ClearIfGray();
                solver.ClearAllMatches();
            }
        }
    }

    private float CheckStillBlocks()
    {
        float stillBlocks = 0F;
        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("gameblock"))
        {
            if (fooObj.name == "Block(Clone)")
            {
                stillBlocks += fooObj.GetComponent<Rigidbody2D>().velocity.magnitude;
            }
            
        }
        return stillBlocks;
    }
}
