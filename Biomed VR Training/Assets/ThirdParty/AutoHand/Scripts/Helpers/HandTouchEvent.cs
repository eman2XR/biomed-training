using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Autohand{
    public class HandTouchEvent : MonoBehaviour{
        [Header("For Solid Collision")]
        [Tooltip("Whether or not first hand to enter should take ownership and be the only one to call events")]
        public bool oneHanded = true;

        [Header("Events")]
        public UnityHandEvent HandStartTouch;
        public UnityHandEvent HandStopTouch;
        
        public HandEvent HandStartTouchEvent;
        public HandEvent HandStopTouchEvent;

        private void OnEnable() {
            hands = new List<Hand>();
            HandStartTouchEvent += (hand) => HandStartTouch?.Invoke(hand);
            HandStopTouchEvent += (hand) => HandStopTouch?.Invoke(hand);
        }

        private void OnDisable() {
            HandStartTouchEvent -= (hand) => HandStartTouch?.Invoke(hand);
            HandStopTouchEvent -= (hand) => HandStopTouch?.Invoke(hand);
        }
        
        List<Hand> hands;

        public void Touch(Hand hand) {
            if(!hands.Contains(hand)) {
                if(oneHanded && hands.Count == 0)
                    HandStartTouchEvent?.Invoke(hand);
                else
                    HandStartTouchEvent?.Invoke(hand);

                hands.Add(hand);
            }
        }
        
        public void Untouch(Hand hand) {
            if(hands.Contains(hand)) {
                if(oneHanded && hands[0] == hand){
                    HandStopTouchEvent?.Invoke(hand);
                    
                    if(hands.Count > 1)
                        HandStartTouchEvent?.Invoke(hand);
                }
                else if(!oneHanded){
                    HandStopTouchEvent?.Invoke(hand);
                }

                hands.Remove(hand);
            }
        }
    }
}
