using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public interface IDamagable
    {
        public TeamID.ID TeamID(); // Returns the team ID
        public void TakeDamage(float damage, Transform damageSource); // Take damage
        public void Death(); // Death
    }
}