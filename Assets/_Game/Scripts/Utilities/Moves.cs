using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public delegate void NoParam();
public delegate float Ease(float param);

public class Moves : MonoBehaviour {
    public static Moves instance;
    public static float DeltaTime;




    private float _lastTime;
    private void Awake() {
        instance = this;
        _lastTime = Time.realtimeSinceStartup;
    }

    public void stopCoroutine(Coroutine c)
    {
        if (c != null)
        {
            StopCoroutine(c);
        }
    }

    private void Update()
    {
//        DeltaTime = Time.realtimeSinceStartup - _lastTime;
//        _lastTime = Time.realtimeSinceStartup;
        DeltaTime = Time.deltaTime;
    }


    public Coroutine startMove(Move move){
        return StartCoroutine(moving(move));
    }

    private Vector3 lerpCurved (Vector3 start, Vector3 end, float par, Move move) {
        float xPar, yPar, zPar;
        
        if (move.curveX != null) {
            xPar = move.curveX.Evaluate(par);
        } else {
            xPar = par;
        }
        
        
        if (move.curveY != null) {
            yPar = move.curveY.Evaluate(par);
        } else {
            yPar = par;
        }
        
        
        if (move.curveZ != null) {
            zPar = move.curveZ.Evaluate(par);
        } else {
            zPar = par;
        }

        float x = start.x + (xPar * (end.x - start.x));
        float y = start.y + (yPar * (end.y - start.y));
        float z = start.z + (zPar * (end.z - start.z));
        return new Vector3(x, y, z);
        
    }
    
    private void moveTo(Move move, float par){
        if (move.obj.obj == null) {
            return;
        }

        
        
        Transform obj = move.obj.obj.transform;
        if (obj != null) {
            if (move.moveType == MoveType.LOCAL)  {
                if(move.localPos) obj.localPosition = lerpCurved(move.obj.startLocalPosition, move.obj.endLocalPosition, par, move);
            } else  if (move.moveType == MoveType.WORLD) {
                if (move.worldPos) obj.position = lerpCurved(move.obj.startPosition, move.obj.endPosition, par, move);
            }
            if (move.scale) obj.localScale = lerpCurved(move.obj.startScale, move.obj.endScale, par, move);
            if (move.rotation)
            {
                float pos = par;
                if (move.curve != null)
                {
                    pos = move.curve.Evaluate(par);
                }
                obj.localRotation = Quaternion.LerpUnclamped(move.obj.startRotation, move.obj.endRotation, pos);
            }

            // if (move.moveType == MoveType.LOCAL)  {
            //     if(move.localPos) obj.localPosition = Vector3.LerpUnclamped(move.obj.startLocalPosition, move.obj.endLocalPosition, par);
            // } else  if (move.moveType == MoveType.WORLD) {
            //     if (move.worldPos) obj.position = Vector3.LerpUnclamped(move.obj.startPosition, move.obj.endPosition, par);
            // }
            // if (move.scale) obj.localScale = Vector3.LerpUnclamped(move.obj.startScale, move.obj.endScale, par);
        }

        RectTransform rect = move.rect.rect;

        if (rect != null) {

            if(move.moveType == MoveType.ANCHORED) if (move.anchPos)  rect.anchoredPosition = Vector2.LerpUnclamped(move.rect.startPosition, move.rect.endPosition, par);

            if (move.anchMin) rect.anchorMin = Vector2.LerpUnclamped(move.rect.startAnchMin, move.rect.endAnchMin, par);
            if (move.anchMax) rect.anchorMax = Vector2.LerpUnclamped(move.rect.startAnchMax, move.rect.endAnchMax, par);
            if (move.pivot) rect.pivot = Vector2.LerpUnclamped(move.rect.startPivot, move.rect.endPivot, par);
            if (move.size) rect.sizeDelta = Vector2.LerpUnclamped(move.rect.startSize, move.rect.endSize, par);
        }
    }

    IEnumerator moving(Move move){
        if (move.delay > 0f) {
            yield return new WaitForSeconds(move.delay);        
        }

        float t = 0f;
        
        
        
        moveTo(move, 0f);

        while (move.animTime >= t) {
            float par =  t / move.animTime;
            if(move.curve != null) par = move.curve.Evaluate(par);
            moveTo(move, par);

            if(!move.paused) t += DeltaTime;
            yield return null;

        }
        
        if (move.curve != null) {
            moveTo(move, move.curve.Evaluate(1f));
        } else {
            moveTo(move, 1f);
        }
        
        if(move.callback != null) move.callback();
    }
    
    // IEnumerator movingUnclamped(Move move) {
    //     if (move.delay > 0f) {
    //         yield return new WaitForSeconds(move.delay);        
    //     }
    //
    //     float t = 0f;
    //     
    //     
    //     
    //     moveTo(move, 0f);
    //
    //     while (move.animTime >= t) {
    //         float par =  t / move.animTime;
    //         par = move.ease(par);
    //         moveTo(move, par);
    //
    //         t += Time.deltaTime;
    //         yield return null;
    //
    //     }
    //     moveTo(move, 1f);
    //     if(move.callback != null) move.callback();
    // }

    public Coroutine executeAfterFrame(NoParam func, int frame = 1)
    {
        return StartCoroutine(frameDelayed(func, frame));

    }

    IEnumerator frameDelayed(NoParam func, int frame)
    {
        for (int i = 0; i < frame; i++)
        {
            yield return null;
        }
        
        func.Invoke();
    }
    public Coroutine executeWithDelay(NoParam func) {
        return StartCoroutine(delayed(func));
    }

    public Coroutine executeWithDelay(NoParam func, float delay){
        return StartCoroutine(delayed(func, delay));
    }

    IEnumerator delayed(NoParam func, float delay) {
//        yield return new WaitForSeconds(delay);
        float t = 0f;
        while (t < delay)
        {
            yield return null;
            t += DeltaTime;
        }
        func();
    }
    
    IEnumerator delayed(NoParam func) {
        yield return null;
        func();
    }


}

public class Move {
    
    
    public delegate float Curve (float f);

    public AnimationCurve curveX, curveY, curveZ;
    
    public AnimationCurve curve;
    
    public ObjectProperties obj;
    public RectProperties rect;
    public MoveType moveType;
    public bool local;
    public bool resetStartValues = true;

    public float animTime;
    public float delay;

    public NoParam callback;

    public Ease ease;


    public bool worldPos, localPos, scale, anchPos, anchMin, anchMax, pivot, size, rotation;

    private Coroutine action;
    public bool paused;

    public Move(GameObject o)
    {

        paused = false;
        
        moveType = MoveType.NONE;

        ease = linearEase;
        local = false;

        obj.obj = o;

        obj.startPosition = o.transform.position;
        obj.endPosition = obj.startPosition;

        obj.startLocalPosition = o.transform.localPosition;
        obj.endLocalPosition = obj.startLocalPosition;

        obj.startScale = o.transform.localScale;
        obj.endScale = obj.startScale;

        obj.startRotation = o.transform.rotation;
        obj.endRotation = o.transform.rotation;


        try {
            rect.rect = o.GetComponent<RectTransform>();

            rect.startSize = rect.rect.sizeDelta;
            rect.endSize = rect.startSize;

            rect.startAnchMin = rect.rect.anchorMin;
            rect.endAnchMin = rect.startAnchMin;

            rect.startAnchMax = rect.rect.anchorMax;
            rect.endAnchMax = rect.startAnchMax;

            rect.startPivot = rect.rect.pivot;
            rect.endPivot = rect.startPivot;

            rect.startPosition = rect.rect.anchoredPosition;
            rect.endPosition = rect.startPosition;

        } catch (System.Exception ex) {
            rect.rect = null;
        }
    }

    public void pause()
    {
        paused = true;
    }

    public void resume()
    {
        paused = false;

    }
    public void start (){
        run();
    }

    public Coroutine run()
    {

//        animTime *= 10;

        rotation = obj.startRotation != obj.endRotation;
        worldPos = obj.startPosition != obj.endPosition;
        localPos = obj.startLocalPosition != obj.endLocalPosition;
        scale = obj.startScale != obj.endScale;

        anchPos = rect.startPosition != rect.endPosition;

        anchMax = rect.startAnchMax != rect.endAnchMax;
        anchMin = rect.startAnchMin != rect.endAnchMin;

        size = rect.startSize != rect.endSize;
        pivot = rect.startPivot != rect.endPivot;

        if (resetStartValues) {
            if (moveType == MoveType.ANCHORED)
            {
                rect.startPosition = rect.rect.anchoredPosition;
                rect.startAnchMax = rect.rect.anchorMax;
                rect.startAnchMin = rect.rect.anchorMin;
                rect.startPivot = rect.rect.pivot;
                rect.startSize = rect.rect.sizeDelta;
                
            }
    
            obj.startPosition = obj.obj.transform.position;
            obj.startLocalPosition = obj.obj.transform.localPosition;
            obj.startScale = obj.obj.transform.localScale;
        }

        action = Moves.instance.startMove(this);
        return action;
    }

    public void stop () {
        if (action != null) {
            Moves.instance.StopCoroutine(action);
        }
    }

    private float linearEase(float par){
        //par = par * par * par * par;
        return par;
    }

}

public struct ObjectProperties{
    public GameObject obj;
    public Vector3 startPosition;
    public Vector3 endPosition;

    public Vector3 startLocalPosition;
    public Vector3 endLocalPosition;

    public Vector3 startScale;
    public Vector3 endScale;

    public Quaternion startRotation;
    public Quaternion endRotation;

}

public struct RectProperties {
    public RectTransform rect;

    public Vector2 startPosition;
    public Vector2 endPosition;

    public Vector2 startAnchMin;
    public Vector2 startAnchMax;
    public Vector2 startPivot;

    public Vector2 endAnchMin;
    public Vector2 endAnchMax;
    public Vector2 endPivot;

    public Vector2 startSize;
    public Vector2 endSize;
}


public class MoveAnimation {
    public List<Move> moves;
    
    public MoveAnimation () {
        moves = new List<Move>();
    }

    public void addSubAnimation (Move m) {
        moves.Add(m);
    }
    public void startAnimation () {
        foreach (var move in moves) {
            move.run();
        }
    }

    public void stopAnimation () {
        foreach (var move in moves) {
            move.stop();
        }
    }
}

public enum MoveType { NONE, WORLD, LOCAL, ANCHORED }

