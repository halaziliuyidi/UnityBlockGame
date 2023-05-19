using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *	
 *  Base View
 *
 *	by Xuanyi
 *
 */

namespace MoleMole
{
	public abstract class BaseView : MonoBehaviour
    {

        public virtual void OnEnter(BaseContext context)
        {
            DebugHelper.LogFormat("{0} on enter",context.ViewType);
        }

        public virtual void OnExit(BaseContext context)
        {
            DebugHelper.LogFormat("{0} on exit", context.ViewType);
        }

        public virtual void OnPause(BaseContext context)
        {
            DebugHelper.LogFormat("{0} on pause", context.ViewType);
        }

        public virtual void OnResume(BaseContext context)
        {
            DebugHelper.LogFormat("{0} on resume", context.ViewType);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
	}
}
