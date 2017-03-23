using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KI {

	private int hitsLeft = 0;
	private int hitsRight = 0;
	private int hitsCenter = 0;
	private int hitsTotal = 0;

	private int speedMin = 1;
	private int speedMax = 10;


	public KI() {

	}

	public KI (List<float[]> col) {

		if (col != null) {
			//determine hits
			foreach (float[] d in col) {
				if (d [1] < -1.5f) {
					hitsLeft++;
				} else if (d [1] >= -1.5f && d [1] < 1.5f) {
					hitsCenter++;
				} else if (d [1] >= 1.5f) {
					hitsRight++;
				}
				hitsTotal++;
			}
			Debug.LogFormat ("KI created, L : {0}, C : {1}, R : {2}, T : {3}", hitsLeft, hitsCenter, hitsRight, hitsTotal);  
	
		} else {
			Debug.Log ("Empty KI created");
		}
	}
		

	//calculate speed adjustment, threshold is amount of collidable objects
	//mind the speed limits
	public int calculateSpeed(int currentSpeed, int threshold) {
		int newSpeed = currentSpeed;

		//too many hits, decrease speed
		if (hitsTotal > threshold && currentSpeed > speedMin) {
			newSpeed--;
			//no hits or less than 5% hit, increase speed
		} else if ((hitsTotal == 0 || hitsTotal < Mathf.RoundToInt (threshold / 5)) 
			&& currentSpeed < speedMax) {
			newSpeed++;
		}

		return newSpeed;
	}

	//probability of empty prefab occuring
	public int calculateEmpty(int collidable) {
		int prob = 0;
		//TODO: formel anpassen für bessere interpolation bei wenig kollisionen
		prob = Mathf.RoundToInt (hitsTotal * 100 / collidable);
		return prob;
	}

	//calculate % of appearance in left, right or center, total ~100
	public int[] calculateDistribution() {
		int[] distro = {0, 0, 0};
		if (hitsTotal != 0) {
			if (hitsLeft != 0) {
				distro [0] = Mathf.RoundToInt (hitsLeft * 100 / hitsTotal);
			}
			if (hitsCenter != 0) {
				distro [1] = Mathf.RoundToInt (hitsCenter * 100 / hitsTotal);
			}
			if (hitsRight != 0) {
				distro [2] = Mathf.RoundToInt (hitsRight * 100 / hitsTotal);
			}

		} else {
			int[] equal = { 33, 33, 33 };
			return equal;
		}

		return distro;
	}

}
