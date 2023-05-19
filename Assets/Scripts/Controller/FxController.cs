using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxController : MonoBehaviour
{
    public GameObject foodObject;

    public GameObject starObject;

    public GameObject destroyVLineFXObject;

    public GameObject destroyHLineFXObject;

    public GameObject createBlockFXObject;

    public GameObject touchFXObject;

    public GameObject comboMatchFXObject;

    public Transform foodTarget;

    public Transform[] starTargets;

    public void SpawnFood(Sprite sprite, Vector3 pos)
    {
        GameObject food_object = TrashMan.spawn(foodObject, pos, foodObject.transform.rotation);
        Food food = food_object.GetComponent<Food>();
        if (food != null)
        {
            food.SetSprite(sprite);
            food.Move(foodTarget.position);
        }
    }

    public void SpawnStar(int index,Sprite sprite,Vector3 pos, Vector3 targetPos)
    {
        GameObject star_object = TrashMan.spawn(starObject, pos, starObject.transform.rotation);
        Star star = star_object.GetComponent<Star>();
        star.index = index;
        if (star != null)
        {
            star.SetSprite(sprite);
            star.Move(targetPos);
        }
    }

    public void SpawnDestroyLineFX(Vector3 pos,float lifeTime=0.1f,bool isV=false)
    {
        GameObject fxObject;
        if (isV)
        {
            fxObject = TrashMan.spawn(destroyVLineFXObject, pos, destroyVLineFXObject.transform.rotation);
        }
        else
        {
            fxObject = TrashMan.spawn(destroyHLineFXObject, pos, destroyHLineFXObject.transform.rotation);
        }
        TrashMan.despawnAfterDelay(fxObject, lifeTime);
    }

    public void SpawnCreateBlockFX(Vector3 pos, float lifeTime = 1f)
    {
        GameObject fxObject= TrashMan.spawn(createBlockFXObject, pos, createBlockFXObject.transform.rotation);
        TrashMan.despawnAfterDelay(fxObject, lifeTime);
    }

    public void SpawnTouchFX(Vector3 pos, float lifeTime = 0.5f)
    {
        GameObject fxObject = TrashMan.spawn(touchFXObject, pos, touchFXObject.transform.rotation);
        TrashMan.despawnAfterDelay(fxObject, lifeTime);
    }

    public void SpawnComboMatchFx(Vector3 pos, float lifeTime = 1f)
    {
        GameObject fxObject = TrashMan.spawn(comboMatchFXObject, pos, comboMatchFXObject.transform.rotation);
        TrashMan.despawnAfterDelay(fxObject, lifeTime);
    }
}
