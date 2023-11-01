using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MoneyBag : MonoBehaviour
{
    [SerializeField] private ParticleSystem takeMoneyParticles;
    public float moneyValue = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<ResourceStorage>().AddCoins(moneyValue);
            Instantiate(takeMoneyParticles, other.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void Drop(Vector3 forceVector)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(forceVector, ForceMode.Impulse);
    }
}
