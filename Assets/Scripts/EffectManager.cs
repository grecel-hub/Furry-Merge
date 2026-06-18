using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum Fruittype
{
    fruit1 = 1, fruit2, fruit3, fruit4, fruit5, fruit6
}
public class Effect_state
{
    public Transform tran_father;public bool isback;public GameObject gam_effect;
    public Fruittype type;
    public Effect_state(Transform tran,GameObject gam,Fruittype type)
    {
        tran_father = tran;isback = false;gam_effect = gam;this.type = type;
    }
}
public class EffectManager:MonoBehaviour
{
    [Header("特效Prefab")]
    private GameObject effectPrefab1;
    private GameObject effectPrefab2;
    private GameObject effectPrefab3;
    private GameObject effectPrefab4;
    private GameObject effectPrefab5;

    [Header("对象池初始数量")]
    [SerializeField] private int poolSize = 10;
    // public Queue<GameObject> pool = new Queue<GameObject>();
    private Dictionary<Fruittype, Queue<Effect_state>> pool = new Dictionary<Fruittype, Queue<Effect_state>>();
    private Dictionary<Fruittype, Transform> trans_ = new Dictionary<Fruittype, Transform>();
    public static EffectManager instance;
    private List<GameObject> effect_list;
    private void Awake()
    {
        instance = this;
        effect_list = new List<GameObject>();
        effectPrefab1 = Resources.Load<GameObject>("effect/hc_1");
        effectPrefab2 = Resources.Load<GameObject>("effect/hc_2");
        effectPrefab3 = Resources.Load<GameObject>("effect/hc_3");
        effectPrefab4 = Resources.Load<GameObject>("effect/hc_4");
        effectPrefab5 = Resources.Load<GameObject>("effect/hc_5");
    }
    //隐藏effect
    public void Gam_end()
    {
        if (effect_list.Count > 0)
        {
            foreach (var temp in effect_list)
            {
                if(temp.activeSelf)
                temp.SetActive(false);
            }
        }
    }
    public Effect_state PlayEffect(Vector3 position,Fruittype type,Transform tran)
    {
        Effect_state state = GetEffect(type,tran);
        state.gam_effect.transform.position = position;
        state.gam_effect.transform.SetParent(tran);
        state.gam_effect.SetActive(true);
        StartCoroutine(RecycleEffect(state.gam_effect,type));
        return state;
    }
    public void Add_(Effect_state state)
    {
        state.gam_effect.transform.SetParent(trans_[state.type]);
        pool[state.type].Enqueue( state);
    }
    private Effect_state GetEffect(Fruittype type,Transform tran)
    {
        Queue<Effect_state> temp;
        if (pool.ContainsKey(type))
        {
            temp = pool[type];
        }
        else
        {
            temp = new Queue<Effect_state>();
            pool[type] = temp;
        }
        if (temp.Count > 0)
        {
            return temp.Dequeue();
        }
        else
        {
            GameObject go=null;
            switch (type)
            {
                case Fruittype.fruit2:go = effectPrefab1;break;
                case Fruittype.fruit3: go = effectPrefab2; break;
                case Fruittype.fruit4: go = effectPrefab3; break;
                case Fruittype.fruit5: go = effectPrefab4; break;
                case Fruittype.fruit6: go = effectPrefab5; break;
            }
            if (!trans_.ContainsKey(type))
            {
                GameObject father = new GameObject(type.ToString());
                father.transform.SetParent(transform);
                trans_[type] = father.transform;
            }
            GameObject gam= Instantiate(go);
            effect_list.Add(go);
            Effect_state state = new Effect_state(trans_[type],gam,type);
            return state;
        }
    }

    private IEnumerator RecycleEffect(GameObject effect,Fruittype type)
    {
        float duration = 1f;

        yield return new WaitForSeconds(duration);
        effect.SetActive(false);
    }
}