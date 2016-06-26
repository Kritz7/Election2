using UnityEngine;
	using System.Collections;

	public class SimpleAnimation : MonoBehaviour {

		public enum AnimationType
		{
			Bobbing,
			MoveToPosition
		}
		public AnimationType animationType;

		public enum LerpStyle
		{
			EaseIn,
			EaseOut,
			Smooth,
			SuperSmooth,
			Linear
		}
		public LerpStyle lerpStyle;

		public enum PositionType
		{
			localPosition,
			worldPosition,
			anchorPosition,
			screenPosition
		}
		public PositionType positionType;

		private Renderer rendererer;

		private float t = 0;
		private Vector3 tV = Vector3.zero;
		private float trueDelta = 0;
		private int currentUpdateFrame = 0;
		private int frameDelay = 0;
		private float distFromCamera = 0;
		private float nearFarPoint;

		public float Duration = 0;

		private Vector3 startPos;
		public Transform EndTransform;
		public Vector3 EndPos;

		public Vector3 BounceMoveSpeed = Vector3.zero;
		public Vector3 BounceFrequency = Vector3.zero;
		public Vector3 BounceFreqOffset = Vector3.zero;

		public Vector3 RotationAxis = Vector3.zero;
		public float RotationSpeed = 0f;

		public bool Billboard = false;
		public bool StopAnimatingWhenLowFramerate = false;
		public bool DestroyOnComplete = true;
		public bool DontChangePos = false;
		RectTransform myRect;

		public bool IgnoreOptimisations = true;
		private float minDistFromCamera = 170f;
		private float minBillboardDistFromCamera = 450f;

		public int added = 0;

		public void InitBounceAnimation(Vector3 bounceMoveSpeed, float rotationSpeed, Vector3 rotationAxis, Vector3 bounceFrequency, Vector3 bounceOffset, float duration = float.PositiveInfinity, bool ignoreOptimisations=true, bool dontChangePos=false)
		{
			switch(positionType)
			{
			case PositionType.anchorPosition:
				myRect = GetComponent<RectTransform>();
				startPos = myRect.anchoredPosition3D;
				break;
			case PositionType.localPosition:
				startPos = transform.localPosition;
				break;
			case PositionType.worldPosition:
				startPos = transform.position;
				break;
			}

			this.animationType = AnimationType.Bobbing;
			this.lerpStyle = LerpStyle.Linear;
			this.Duration = duration;
			this.BounceMoveSpeed = bounceMoveSpeed;
			this.RotationSpeed = rotationSpeed;
			this.RotationAxis = rotationAxis;
			this.BounceFrequency = bounceFrequency;
			this.BounceFreqOffset = bounceOffset;
			this.IgnoreOptimisations = ignoreOptimisations;
			this.DontChangePos = dontChangePos;
			this.DestroyOnComplete = false;

			ResetT();
		}

		public void InitMoveAnimation(Transform endTransform, LerpStyle lerpStyle, float duration, bool ignoreOptimisations=true, PositionType posType = PositionType.localPosition, bool destroyOnComplete=true)
		{
			this.positionType = posType;

			switch(posType)
			{
			case PositionType.anchorPosition:
				myRect = GetComponent<RectTransform>();
				startPos = myRect.anchoredPosition3D;
				myRect.anchoredPosition3D = startPos;
				break;
			case PositionType.screenPosition:
				this.startPos = transform.position;
				this.EndPos = Camera.main.ScreenToWorldPoint(new Vector3(endTransform.transform.position.x, endTransform.transform.position.y, nearFarPoint));
				break;
			case PositionType.localPosition:
				this.startPos = transform.localPosition;
				break;
			case PositionType.worldPosition:
				this.startPos = transform.position;
				break;
			}

			this.EndTransform = endTransform;
			this.animationType = AnimationType.MoveToPosition;
			this.lerpStyle = lerpStyle;
			this.Duration = duration;
			this.IgnoreOptimisations = ignoreOptimisations;
			this.DestroyOnComplete = destroyOnComplete;

			ResetT();
		}

		public void InitMoveAnimation(Vector3 endPos, LerpStyle lerpStyle, float duration, bool ignoreOptimisations=true, PositionType posType = PositionType.localPosition, bool destroyOnComplete=true)
		{
			this.positionType = posType;

			switch(posType)
			{
			case PositionType.anchorPosition:
				myRect = GetComponent<RectTransform>();
				startPos = myRect.anchoredPosition3D;
				myRect.anchoredPosition3D = startPos;
				break;
			case PositionType.localPosition:
				this.startPos = transform.localPosition;
				break;
			case PositionType.worldPosition:
				this.startPos = transform.position;
				break;
			}
				
			this.EndPos = endPos;
			this.animationType = AnimationType.MoveToPosition;
			this.lerpStyle = lerpStyle;
			this.Duration = duration;
			this.IgnoreOptimisations = ignoreOptimisations;
			this.DestroyOnComplete = destroyOnComplete;

			ResetT();
		}

		public void InitMoveAnimation(Vector3 startPos, Vector3 endPos, LerpStyle lerpStyle, float duration, bool ignoreOptimisations=true, PositionType posType = PositionType.localPosition, bool destroyOnComplete=true)
		{
			this.positionType = posType;

			switch(positionType)
			{
			case PositionType.anchorPosition:
				myRect = GetComponent<RectTransform>();
				startPos = myRect.anchoredPosition3D;
				myRect.anchoredPosition3D = startPos;
				break;
			default:
				this.startPos = startPos;
				break;
			}			

			this.EndPos = endPos;
			this.animationType = AnimationType.MoveToPosition;
			this.lerpStyle = lerpStyle;
			this.Duration = duration;
			this.IgnoreOptimisations = ignoreOptimisations;
			this.DestroyOnComplete = destroyOnComplete;
			
			ResetT();

		}
			
		public void InitScale(float startScaleMag, float endScaleMag, LerpStyle style, float duration)
		{
			startPos = transform.position;
			Vector3 endScale = new Vector3(1,1,1) * endScaleMag;
			StartCoroutine(Scale(new Vector3(1,1,1) * startScaleMag, endScale, style, duration));
		}

		private IEnumerator Scale(Vector3 startScale, Vector3 endScale, LerpStyle style, float duration)
		{
			float myT = 0;
			float completion = 0;

			while(myT < duration)
			{
				myT += Time.deltaTime;
				completion = GetCompletion(myT, duration, style);

				transform.localScale = Vector3.Lerp(startScale, endScale, completion);

				yield return 0;
			}

			transform.localScale = endScale;

			Destroy(this);
		}

		private float GetCompletion(float t, float duration, LerpStyle style)
		{
			float completion = t / duration;

			switch(style)
			{
			case LerpStyle.EaseIn:
				return Mathf.Sin(completion * Mathf.PI * 0.5f);
			case LerpStyle.EaseOut:
				return 1f - Mathf.Cos(completion * Mathf.PI * 0.5f);
			case LerpStyle.Smooth:
				return completion * completion * (3f - 2f * completion);
			case LerpStyle.SuperSmooth:
				return completion * completion * completion * (completion * (6f * completion - 15f) + 10f);
			default:
				return completion;
			}
		}

		void OnDisable()
		{
			if(added!=-2)
			{
				added = -1;	
				if(animationType == AnimationType.Bobbing)
				{
					switch(positionType)
					{
					case PositionType.anchorPosition:
						myRect.anchoredPosition3D = startPos;
						break;
					case PositionType.localPosition:
						transform.localPosition = startPos;
						break;
					case PositionType.worldPosition:
						transform.position = startPos;
						break;
					}
				}
            }
		}

		void Start()
		{
			if(animationType == AnimationType.Bobbing)
			{

				InitBounceAnimation(BounceMoveSpeed, RotationSpeed, RotationAxis, BounceFrequency, BounceFreqOffset, (Duration==0?float.PositiveInfinity:Duration), IgnoreOptimisations, DontChangePos);
			}
			
			if(animationType == AnimationType.MoveToPosition)
			{
				if(startPos!=Vector3.zero)
				{
					InitMoveAnimation(startPos, EndPos, lerpStyle, Duration, IgnoreOptimisations, positionType, DestroyOnComplete);
				}
				else if(EndTransform!=null)
				{
					InitMoveAnimation(EndTransform, lerpStyle, Duration, IgnoreOptimisations, positionType, DestroyOnComplete);
				}
				else
				{
					InitMoveAnimation(EndPos, lerpStyle, Duration, IgnoreOptimisations, positionType, DestroyOnComplete);
				}
			}

			if(Camera.main != null)
				nearFarPoint = Mathf.Lerp(Camera.main.nearClipPlane, Camera.main.farClipPlane, 0.2f);
			
			rendererer = GetComponentInChildren<Renderer>();
			StartCoroutine(DistFromCam());
		}

		void ResetT()
		{
			t = 0;
			tV = Vector3.zero;
		}

		IEnumerator DistFromCam()
		{
			while(true)
			{
				if(Camera.main)
					distFromCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
				else
					distFromCamera = 1;

				yield return new WaitForSeconds(Random.Range(0.1f,1f));
			}
		}
			
		void Update ()
		{
			if((rendererer==null || (rendererer!=null && !rendererer.isVisible)) && !IgnoreOptimisations)
			{
				// Can't see object, don't update
				frameDelay = 60;
			}
			else if((1/Time.deltaTime)<20f && !IgnoreOptimisations)
			{
				if(StopAnimatingWhenLowFramerate)
				{
					trueDelta = 0;
					return;
				}

				// Can see object, framerate is low, lower animation framerate based on distance
				if(distFromCamera<minDistFromCamera * 2f)
				{
					frameDelay = 2;
				}
				else
				{
					if(distFromCamera < minDistFromCamera * 5f)
					{
						frameDelay = 5;
					}
					else if(distFromCamera < minDistFromCamera * 8f)
					{
						frameDelay = 10;
					}
					else
					{
						frameDelay = 20;
					}
				}
			}
			else
			{
				frameDelay = 0;
			}

			trueDelta += Time.deltaTime;
			currentUpdateFrame++;

			if(currentUpdateFrame < frameDelay)
				return;
			
			// BOBBING ANIMATION
			if(animationType == AnimationType.Bobbing)
			{
				tV += new Vector3(trueDelta * BounceMoveSpeed.x, trueDelta * BounceMoveSpeed.y, trueDelta * BounceMoveSpeed.z);
				Vector3 newPos = new Vector3(
					Mathf.Sin(tV.x) * BounceFrequency.x,
					Mathf.Sin(tV.y) * BounceFrequency.y,
					Mathf.Sin(tV.z) * BounceFrequency.z);

				if(!DontChangePos)
				{
					switch(positionType)
					{
					case PositionType.anchorPosition:
						myRect.anchoredPosition3D = new Vector3(
							startPos.x + BounceFreqOffset.x + newPos.x,
							startPos.y + BounceFreqOffset.y + newPos.y,
							startPos.z + BounceFreqOffset.z + newPos.z);
						break;
					case PositionType.localPosition:
						transform.localPosition = new Vector3(
							startPos.x + BounceFreqOffset.x + newPos.x,
							startPos.y + BounceFreqOffset.y + newPos.y,
							startPos.z + BounceFreqOffset.z + newPos.z);
						break;
					default:
						transform.position = new Vector3(
							startPos.x + BounceFreqOffset.x + newPos.x,
							startPos.y + BounceFreqOffset.y + newPos.y,
							startPos.z + BounceFreqOffset.z + newPos.z);
						break;
					}
				}

				if(RotationAxis != Vector3.zero && RotationSpeed != 0)
				{
					transform.Rotate(RotationAxis, RotationSpeed * trueDelta);
				}
			}

			// MOVE TO LOCATION AND THEN DESTROY... ANIMATION
			if(animationType == AnimationType.MoveToPosition)
			{
				if(Duration == 0)
				{
					Duration = 0.01f;
				}

				if(t < Duration)
				{
					t += trueDelta;
					float completion = t / Duration;

					switch(lerpStyle)
					{
					case LerpStyle.EaseIn:
						completion = Mathf.Sin(completion * Mathf.PI * 0.5f);
						break;
					case LerpStyle.EaseOut:
						completion = 1f - Mathf.Cos(completion * Mathf.PI * 0.5f);
						break;
					case LerpStyle.Smooth:
						completion = completion * completion * (3f - 2f * completion);
						break;
					case LerpStyle.SuperSmooth:
						completion = completion * completion * completion * (completion * (6f * completion - 15f) + 10f);
						break;
					}

					if(EndTransform!=null) EndPos = EndTransform.position;

					switch(positionType)
					{
					case PositionType.anchorPosition:
						myRect.anchoredPosition3D = Vector3.Lerp(startPos, EndPos, completion);
						break;
					case PositionType.screenPosition:
						EndPos = Camera.main.ScreenToWorldPoint(new Vector3(EndTransform.transform.position.x, EndTransform.transform.position.y, nearFarPoint));
						transform.position = Vector3.Lerp(startPos, EndPos, completion);
						break;
					case PositionType.localPosition:
						transform.localPosition = Vector3.Lerp(startPos, EndPos, completion);
						break;
					case PositionType.worldPosition:
						transform.position = Vector3.Lerp(startPos, EndPos, completion);
						break;
					}
				}

				if(RotationAxis!=Vector3.zero) transform.Rotate(RotationAxis, RotationSpeed * trueDelta);

				if(t >= Duration)
				{
					switch(positionType)
					{
					case PositionType.anchorPosition:
						myRect.anchoredPosition3D = EndPos;
						break;
					case PositionType.screenPosition:
						EndPos = Camera.main.ScreenToWorldPoint(new Vector3(EndTransform.transform.position.x, EndTransform.transform.position.y, nearFarPoint));
						transform.position = EndPos;
						break;
					case PositionType.localPosition:
						transform.localPosition = EndPos;
						break;
					case PositionType.worldPosition:
						transform.position = EndPos;
						break;
					}

					if(DestroyOnComplete)
					{
						Destroy(this);
					}
				}
			}

			trueDelta = 0;
			currentUpdateFrame = 0;
			if((rendererer!=null && rendererer.isVisible && distFromCamera < minBillboardDistFromCamera) || IgnoreOptimisations )
			{
				if(Billboard)
					transform.LookAt(Camera.main.transform);
			}
		}
		
		public bool WithinDistance(float dist)
		{
			if(rendererer!=null && !rendererer.isVisible)
			{
				if(Vector3.Distance(Camera.main.transform.position, transform.position)<dist)
					return true;
			}
			
			return false;
		}
	}