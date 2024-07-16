using console_demo;
using NSubstitute;
using Shouldly;
using System.Collections;

namespace test_demo
{
    public class OrderProcessor_Test
    {
        #region Dùng [Fact]

        //testcase kiểm tra trường hợp null
        [Fact]
        public void Test_OrderProcess_Null_Order()
        {
            //Tạo mới đối tượng processor với các tham số null
            OrderProcessor processor = new OrderProcessor(null, null, null, null);

            //Dùng should.Thorw để kiểm tra exception trả ra
            Should.Throw<ArgumentException>(() => processor.Process(null));
        }


        //Testcase kiểm tra invalid
        [Fact]
        public void Test_OrderProcess_Invalid_Order()
        {
            //khởi tạo đối tượng mock validator bằng Substitute.For<T>()
            var validator = Substitute.For<IOrderValidator>();
            //quy định giá trị trả về cho hàm Validate() bằng hàm Returns()
            validator.Validate(Arg.Any<Order>()).Returns(false);

            //Khởi tạo đối tượng mock inventoryProvider bằng Substitute.For<T>()
            var inventoryProvider = Substitute.For<IIventoryProvider>();
            //quy định giá trị trả về cho hàm Validate() bằng hàm Returns()
            inventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(false);

            //Tạo mới đối tượng OrderProcessor với 2 đối tượng mock ở trên
            var process = new OrderProcessor(validator, inventoryProvider, null, null);
            //Kiểm tra exception trả về bằng hàm Should.Throw => Nếu k có exception thì là lỗi
            Should.Throw<Exception>(() => process.Process(new()));

            //Xác nhận lại phương thức Received() đã được gọi => Nếu chưa đc gọi sẽ là lỗi
            var valResult = validator.Received().Validate(Arg.Any<Order>());

            //Xác nhận lại phương thức DidNotReceive() chưa được gọi => Nếu đã đc gọi sẽ là lỗi
            var inventoryResult = inventoryProvider.DidNotReceive().IsAvailable(Arg.Any<Order>());
            //Lưu ý: 2 hàm Received() và DidNotReceive() chỉ là xác nhận lại. Nó không kiểm tra giá trị là đúng hay là sai nào cả
        }

        //Testcase trường hợp không có hàng tồn kho
        [Fact]
        public void Test_OrderProcess_No_Inventory()
        {
            //tạo mới đối tượng mock IOrderValidator bằng Substitute.For<T>
            var validator = Substitute.For<IOrderValidator>();
            //thiết lập giá trị trả về mong muốn của hàm
            //mong muốn giá trị true để không bị lỗi
            validator.Validate(Arg.Any<Order>()).Returns(true);

            //tạo mới đối tượng mock IIventoryProvider bằng Substitute.For<T>
            var inventoryProvider = Substitute.For<IIventoryProvider>();
            //thiết lập giá trị trả về mong muốn của hàm
            //mong muốn giá trị false để trả về lỗi để test trường hợp k có hàng tồn kho
            inventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(false);

            //khởi tạo đối tượng OrderProcessor và truyền các tham số đối tượng đã mock ở trên
            var process = new OrderProcessor(validator, inventoryProvider, null, null);
            //Kiểm tra trường hợp Exception khi chạy hàm => bắt buộc chạy ra exception khi k có hàng tồn kho thì mới pass
            Should.Throw<Exception>(() => process.Process(new()));

            //xác nhận lại các hàm của đối tượng mock đã được gọi hay chưa?
            validator.Received().Validate(Arg.Any<Order>());
            inventoryProvider.Received().IsAvailable(Arg.Any<Order>());
        }

        //testcase kiểm tra trường hợp order thất bại
        [Fact]
        public void Test_OrderProcess_Fail_Order()
        {
            //tạo mới đối tượng mock IOrderValidator bằng Substitute.For
            var validator = Substitute.For<IOrderValidator>();
            //thiết lập giá trị mong muốn của hàm
            //mong muốn trả về true
            validator.Validate(Arg.Any<Order>()).Returns(true);

            //tạo mới đối tượng mock IIventoryProvider bằng Substitute.For
            var inventoryProvider = Substitute.For<IIventoryProvider>();
            //thiết lập giá trị mong muốn của hàm
            //mong muốn trả về true
            inventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(true);

            //tạo mới đối tượng mock IOrderCreator bằng Substitute.For
            var orderCreator = Substitute.For<IOrderCreator>();
            //thiết lập giá trị mong muốn của hàm
            //ReturnsForAnyArgs: luôn trả về giá trị khi truyền bất cứ arg nào vào
            //trả về false
            orderCreator.CreateOrder(default).ReturnsForAnyArgs(false);

            //Khởi tạo đối tượng OrderProcessor và truyền các đối tượng đã tạo mock vào
            var process = new OrderProcessor(validator, inventoryProvider, orderCreator, null);
            //chạy hàm và kiểm tra kết quả trả về của hàm có là false không bằng hàm ShouldBeFalse() 
            process.Process(new()).ShouldBeFalse();

            //Xác nhận lại các hàm đã chạy của đối tượng mock
            validator.Received().Validate(Arg.Any<Order>());
            inventoryProvider.Received().IsAvailable(Arg.Any<Order>());
            orderCreator.Received().CreateOrder(Arg.Any<Order>());
        }

        //testcase kiểm tra trường hợp thành công
        [Fact]
        public void Test_OrderProcess_Success_Order()
        {
            //tạo mới đối tượng mock IOrderValidator bằng Substitute.For
            var validator = Substitute.For<IOrderValidator>();
            //thiết lập giá trị trả về mong muốn của hàm
            //trả về true
            validator.Validate(Arg.Any<Order>()).Returns(true);

            //tạo mới đối tượng IIventoryProvider bằng Substitute.For
            var inventoryProvider = Substitute.For<IIventoryProvider>();
            //thiết lập giá trị trả về mong muốn của hàm
            //trả về true
            inventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(true);

            //tạo mới đối tượng IOrderCreator bằng Substitute.For
            var orderCreator = Substitute.For<IOrderCreator>();
            //thiết lập giá trị trả về mong muốn của hàm
            //trả về true
            orderCreator.CreateOrder(default).ReturnsForAnyArgs(true);

            //tạo mới đối tượng INotificationSender bằng Substtitute.For
            //làm hàm void nên không cần trả về giá trị mong muốn
            var emailSender = Substitute.For<INotificationSender>();

            //khởi tạo OrderProcessor và truyền vào các đối tượng đã tạo mock ở trên
            var process = new OrderProcessor(validator, inventoryProvider, orderCreator, emailSender);
            //chạy hàm
            var result = process.Process(new());
            //kiểm tra hàm có về giá trị true hay không bằng hàm ShouldBeTrue()
            result.ShouldBeTrue();

            //xác nhận lại các hàm đã được gọi
            validator.Received().Validate(Arg.Any<Order>());
            inventoryProvider.Received().IsAvailable(Arg.Any<Order>());
            orderCreator.Received().CreateOrder(Arg.Any<Order>());
            emailSender.Received().SendNotification(Arg.Any<Order>());
        }
        #endregion


        #region Dùng [Theory] [ClassData(typeof(T))]
        //Dùng ClassData để truyền các trường hợp theo dạng class
        //Tạo ra class chứa các trường hợp
        class DataTest_OrderProcess : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                //kiểm tra trường hợp tất cả đều null
                yield return new object[]
                {
                    null,
                    null,
                    null,
                    null,
                    "ArgumentException"
                };
                //kiểm tra trường hợp invalid
                yield return new object[]
                {
                    Substitute.For<IOrderValidator>(),
                    null,
                    null,
                    null,
                    "Exception"
                };
                //kiểm tra trường hợp không có hàng tồn kho
                yield return new object[]
                {
                    Substitute.For<IOrderValidator>(),
                    Substitute.For<IIventoryProvider>(),
                    null,
                    null,
                    "Exception"
                };
                //kiểm tra trường hợp order fail
                yield return new object[]
                {
                    Substitute.For < IOrderValidator >(),
                    Substitute.For < IIventoryProvider >(),
                    Substitute.For < IOrderCreator >(),
                    null,
                    "false"
                };
                //kiểm tra trường hợp thành công
                yield return new object[]
                {
                    Substitute.For < IOrderValidator >(),
                    Substitute.For < IIventoryProvider>(),
                    Substitute.For < IOrderCreator >(),
                    Substitute.For<INotificationSender>(),
                    "true"
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(DataTest_OrderProcess))]
        public void Test_OrderProcess_ClassData(IOrderValidator orderValidator, IIventoryProvider iventoryProvider, IOrderCreator orderCreator, INotificationSender notificationSender, string type)
        {
            //khởi tạo OrderProcessor và truyền vào các đối tượng đã tạo mock ở trên
            var process = new OrderProcessor(orderValidator, iventoryProvider, orderCreator, notificationSender);

            //Dùng should.Thorw để kiểm tra exception trả ra
            if (type == "ArgumentException") Should.Throw<ArgumentException>(() => process.Process(null));

            //Kiểm tra trường hợp Exception khi chạy hàm => bắt buộc chạy ra exception khi k có hàng tồn kho thì mới pass
            else if (type == "Exception")
            {

                if (iventoryProvider != null)
                {
                    orderValidator.Validate(Arg.Any<Order>()).Returns(true);
                    iventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(false);
                }
                else { orderValidator.Validate(Arg.Any<Order>()).Returns(false); }
                Should.Throw<Exception>(() => process.Process(new()));
            }

            //chạy hàm và kiểm tra kết quả trả về của hàm có là false không bằng hàm ShouldBeFalse() 
            else if (type == "false")
            {
                iventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(true);
                orderValidator.Validate(Arg.Any<Order>()).Returns(true);
                orderCreator.CreateOrder(Arg.Any<Order>()).Returns(false);
                process.Process(new()).ShouldBeFalse();
            }

            //kiểm tra hàm có về giá trị true hay không bằng hàm ShouldBeTrue()
            else if (type == "true")
            {
                iventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(true);
                orderValidator.Validate(Arg.Any<Order>()).Returns(true);
                orderCreator.CreateOrder(Arg.Any<Order>()).Returns(true);
                process.Process(new()).ShouldBeTrue();
            }

            //xác nhận lại các hàm đã được gọi
            if (orderValidator != null) orderValidator.Received().Validate(Arg.Any<Order>());
            if (iventoryProvider != null) iventoryProvider.Received().IsAvailable(Arg.Any<Order>());
            if (orderCreator != null) orderCreator.Received().CreateOrder(Arg.Any<Order>());
            if (notificationSender != null) notificationSender.Received().SendNotification(Arg.Any<Order>());
        }
        #endregion

        #region Dùng [Theory] [ClassData(typeof(T))]
        //Dùng MemberData để truyền các trường hợp theo dạng enumerable
        //Tạo ra ds enumerable<object[]> chứa các trường hợp
        public static IEnumerable<object[]> member = new[]
        { 
                //kiểm tra trường hợp tất cả đều null
                new object[]
                {
                    null,
                    null,
                    null,
                    null,
                    "ArgumentException"
                },
                //kiểm tra trường hợp invalid
                new object[]
                {
                    Substitute.For<IOrderValidator>(),
                    null,
                    null,
                    null,
                    "Exception"
                },
                //kiểm tra trường hợp không có hàng tồn kho
                 new object[]
                {
                    Substitute.For<IOrderValidator>(),
                    Substitute.For<IIventoryProvider>(),
                    null,
                    null,
                    "Exception"
                },
                //kiểm tra trường hợp order fail
                new object[]
                {
                    Substitute.For < IOrderValidator >(),
                    Substitute.For < IIventoryProvider >(),
                    Substitute.For < IOrderCreator >(),
                    null,
                    "false"
                },
                //kiểm tra trường hợp thành công
                new object[]
                {
                    Substitute.For < IOrderValidator >(),
                    Substitute.For < IIventoryProvider>(),
                    Substitute.For < IOrderCreator >(),
                        Substitute.For<INotificationSender>(),
                    "true"
                }
        };

        [Theory]
        [MemberData(nameof(member))]
        public void Test_OrderProcess_MemberData(IOrderValidator orderValidator, IIventoryProvider iventoryProvider, IOrderCreator orderCreator, INotificationSender notificationSender, string type)
        {
            //khởi tạo OrderProcessor và truyền vào các đối tượng đã tạo mock ở trên
            var process = new OrderProcessor(orderValidator, iventoryProvider, orderCreator, notificationSender);

            //Dùng should.Thorw để kiểm tra exception trả ra
            if (type == "ArgumentException") Should.Throw<ArgumentException>(() => process.Process(null));

            //Kiểm tra trường hợp Exception khi chạy hàm => bắt buộc chạy ra exception khi k có hàng tồn kho thì mới pass
            else if (type == "Exception")
            {

                if (iventoryProvider != null)
                {
                    orderValidator.Validate(Arg.Any<Order>()).Returns(true);
                    iventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(false);
                }
                else { orderValidator.Validate(Arg.Any<Order>()).Returns(false); }
                Should.Throw<Exception>(() => process.Process(new()));
            }

            //chạy hàm và kiểm tra kết quả trả về của hàm có là false không bằng hàm ShouldBeFalse() 
            else if (type == "false")
            {
                iventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(true);
                orderValidator.Validate(Arg.Any<Order>()).Returns(true);
                orderCreator.CreateOrder(Arg.Any<Order>()).Returns(false);
                process.Process(new()).ShouldBeFalse();
            }

            //kiểm tra hàm có về giá trị true hay không bằng hàm ShouldBeTrue()
            else if (type == "true")
            {
                iventoryProvider.IsAvailable(Arg.Any<Order>()).Returns(true);
                orderValidator.Validate(Arg.Any<Order>()).Returns(true);
                orderCreator.CreateOrder(Arg.Any<Order>()).Returns(true);
                process.Process(new()).ShouldBeTrue();
            }

            //xác nhận lại các hàm đã được gọi
            if (orderValidator != null) orderValidator.Received().Validate(Arg.Any<Order>());
            if (iventoryProvider != null) iventoryProvider.Received().IsAvailable(Arg.Any<Order>());
            if (orderCreator != null) orderCreator.Received().CreateOrder(Arg.Any<Order>());
            if (notificationSender != null) notificationSender.Received().SendNotification(Arg.Any<Order>());
        }
        #endregion
    }

}