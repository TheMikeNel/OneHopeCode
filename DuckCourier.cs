using UnityEngine;

public class DuckCourier : MonoBehaviour
{
    [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject endPosition;
    [SerializeField] private GameObject backpack;
    [SerializeField] private float timeToDelivery;
    [SerializeField] private GameObject resourcesBagPrefab;
    [SerializeField] private GameObject moneyBagPrefab;
    [SerializeField] private Vector3 forceToDropMoney;

    private GameObject _currentPack;
    private Vector3 _currentPosition;
    private Animator _anim;
    private bool _isFlightBack = false;
    private bool _isFlight = false;
    private float _t = 0;
    private float _money = 0;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isFlight)
        {
            DeliveryTimer();
            DeliveryMovement();
        }
    }

    private void DeliveryMovement()
    {
        float speed = (_currentPosition - transform.position).normalized.magnitude / timeToDelivery;
        transform.position = Vector3.MoveTowards(transform.position, _currentPosition, speed);
        transform.LookAt(_currentPosition);
    }
    private void DeliveryTimer()
    {
        _t += Time.deltaTime;

        if ((_t >= timeToDelivery / 2) && !_isFlightBack)
        {
            _currentPosition = startPosition.transform.position;
            Destroy(_currentPack);
            _currentPack = Instantiate(moneyBagPrefab, backpack.transform);
            _currentPack.GetComponent<MoneyBag>().moneyValue = _money;
            _t = 0;
            _isFlightBack = true;
        }

        if ((_t >= timeToDelivery / 2) && _isFlightBack)
        {
            _anim.SetBool("IsFlight", false);

            _currentPack.transform.SetParent(startPosition.transform);
            _currentPack.GetComponent<MoneyBag>().Drop(forceToDropMoney);

            _t = 0;

            _isFlightBack = false;
            _isFlight = false;
        }
    }
    public void SendCourier(float deliveryTime, float deliveryResourcesCost)
    {
        _anim.SetBool("IsFlight", true);

        timeToDelivery = deliveryTime;
        _money = deliveryResourcesCost;

        _currentPosition = endPosition.transform.position;
        _currentPack = Instantiate(resourcesBagPrefab, backpack.transform);

        _isFlight = true;

        DeliveryMovement();
    }
}
