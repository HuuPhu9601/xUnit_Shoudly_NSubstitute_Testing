namespace console_demo
{
    public class OrderProcessor
    {
        IOrderValidator _orderValidator;
        IIventoryProvider _iventoryProvider;
        IOrderCreator _orderCreator;
        INotificationSender _notificationSender;

        public OrderProcessor(IOrderValidator orderValidator, IIventoryProvider iventoryProvider, IOrderCreator orderCreator, INotificationSender notificationSender)
        {
            _orderValidator = orderValidator;
            _orderCreator = orderCreator;
            _iventoryProvider = iventoryProvider;
            _notificationSender = notificationSender;

        }

        public bool Process(Order order)
        {
            if (order == null) throw new ArgumentNullException();

            if (!_orderValidator.Validate(order)) throw new Exception("Invalid");

            if (!_iventoryProvider.IsAvailable(order)) throw new Exception("Missing inventory");

            if (!_orderCreator.CreateOrder(order)) return false;

            _notificationSender.SendNotification(order);
            return true;
        }
    }
}
