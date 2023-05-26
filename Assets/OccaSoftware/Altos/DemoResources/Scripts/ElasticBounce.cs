using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OccaSoftware.Altos.Demo
{
	/// <summary>
	/// Used to oscillate a game object between it's original transform.position and +/- distance units.
	/// </summary>
	public class ElasticBounce : MonoBehaviour
	{
		public Vector3 speed = new Vector3(1,0,0);
		public Vector3 distance = new Vector3(400, 400, 400);
		private Vector3 posStart;

		private void Start()
		{
			posStart = transform.localPosition;
		}

		void Update()
		{
			transform.localPosition = GetNewPosition();
		}

		/// <summary>
		/// Calculates the new object transform.position based on the speed and distance parameters according to a sine wave.
		/// </summary>
		/// <returns>
		/// The new object transform.position Vector3.
		/// </returns>
		Vector3 GetNewPosition()
		{
			return new Vector3(posStart.x + Mathf.Sin(Time.time * speed.x) * distance.x, posStart.y + Mathf.Sin(Time.time * speed.y) * distance.y, posStart.z + Mathf.Sin(Time.time * speed.z) * distance.z);
		}
	}

}