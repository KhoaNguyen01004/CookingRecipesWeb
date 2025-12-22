# Các Trường hợp Sử dụng cho Ứng dụng Web Công thức Nấu ăn

## Các Trường hợp Sử dụng Chức năng

| ID | Mô tả | Yêu cầu Liên quan |
|----|--------|-------------------|
| UC-001 | Cho phép người dùng mới tạo tài khoản bằng cách cung cấp tên, họ, email và mật khẩu. Hệ thống xác thực đầu vào, tạo hồ sơ người dùng trong cơ sở dữ liệu và tự động đăng nhập người dùng. | Xác thực người dùng, xác thực dữ liệu |
| UC-002 | Cho phép người dùng hiện tại đăng nhập bằng email và mật khẩu. Hệ thống xác thực thông tin đăng nhập, thiết lập dữ liệu phiên và chuyển hướng đến trang chủ. | Xác thực người dùng, quản lý phiên |
| UC-003 | Hiển thị danh sách công thức, có thể ngẫu nhiên hoặc được lọc theo danh mục. Người dùng có thể xem các công thức ban đầu và tải thêm khi cần. | Hiển thị công thức, phân trang |
| UC-004 | Hiển thị thông tin chi tiết về một công thức cụ thể, bao gồm nguyên liệu, hướng dẫn và danh mục. | Hiển thị thông tin công thức |
| UC-005 | Cho phép người dùng tìm kiếm công thức theo tên hoặc từ khóa, hiển thị kết quả phù hợp. | Chức năng tìm kiếm |
| UC-006 | Liệt kê tất cả các danh mục công thức có sẵn để duyệt. | Hiển thị danh mục |
| UC-007 | Cho phép người dùng đã đăng nhập thêm hoặc xóa công thức khỏi danh sách yêu thích. | Cá nhân hóa người dùng, quản lý yêu thích |
| UC-008 | Hiển thị các công thức yêu thích của người dùng trên một trang riêng. | Cá nhân hóa người dùng, hiển thị yêu thích |
| UC-009 | Tải thêm công thức cho phân trang trong chế độ xem danh mục. | Phân trang, hiệu suất |
| UC-010 | Chuyển hướng đến trang chi tiết của một công thức ngẫu nhiên. | Lựa chọn ngẫu nhiên |

### Giải thích Chi tiết

**UC-001: Đăng ký Người dùng**  
Trường hợp sử dụng này bao gồm quá trình giới thiệu người dùng ban đầu. Người dùng điền biểu mẫu đăng ký với thông tin cá nhân. Hệ thống thực hiện xác thực (ví dụ: tính duy nhất của email, độ mạnh của mật khẩu). Sau khi đăng ký thành công, bản ghi người dùng được chèn vào cơ sở dữ liệu Supabase, và người dùng được xác thực tự động với phiên được tạo. Điều này cho phép truy cập ngay lập tức vào các tính năng cá nhân hóa như yêu thích.

**UC-002: Đăng nhập Người dùng**  
Người dùng hiện tại truy cập biểu mẫu đăng nhập để nhập thông tin đăng nhập của họ. Hệ thống xác minh email và mật khẩu so với cơ sở dữ liệu Supabase. Nếu hợp lệ, một phiên được thiết lập với thông tin người dùng được lưu trữ, cho phép truy cập các tính năng được bảo vệ. Các lần thử không hợp lệ hiển thị thông báo lỗi mà không tiết lộ thông tin nhạy cảm.

**UC-003: Duyệt Công thức**  
Trang chủ hiển thị công thức dựa trên lựa chọn của người dùng. Công thức ngẫu nhiên được tìm nạp ban đầu, hoặc công thức cụ thể theo danh mục nếu một danh mục được chọn. Phân trang cho phép tải thêm công thức qua AJAX để cải thiện hiệu suất. Bộ nhớ đệm được sử dụng để lưu trữ kết quả tạm thời.

**UC-004: Xem Chi tiết Công thức**  
Người dùng nhấp vào một công thức để xem chi tiết đầy đủ. Hệ thống tìm nạp dữ liệu công thức từ API MealDB bằng ID công thức. Thông tin bao gồm danh sách nguyên liệu, hướng dẫn từng bước, thời gian chuẩn bị, khẩu phần và danh mục. Trang này cũng bao gồm chuyển đổi yêu thích cho người dùng đã đăng nhập.

**UC-005: Tìm kiếm Công thức**  
Người dùng nhập các từ khóa tìm kiếm vào thanh tìm kiếm. Hệ thống truy vấn API MealDB để tìm công thức phù hợp với tiêu chí tìm kiếm. Kết quả được hiển thị ở định dạng danh sách tương tự như chế độ xem duyệt. Nếu không tìm thấy kết quả, một thông báo sẽ được hiển thị.

**UC-006: Xem Danh mục**  
Trang danh mục hiển thị tất cả các danh mục bữa ăn từ API MealDB. Mỗi danh mục được hiển thị dưới dạng mục có thể nhấp để lọc công thức khi được chọn. Điều này cung cấp một cách có tổ chức để người dùng khám phá công thức theo loại (ví dụ: Ý, Trung Quốc, Tráng miệng).

**UC-007: Thêm/Xóa Yêu thích**  
Người dùng đã xác thực có thể đánh dấu công thức là yêu thích từ trang chi tiết công thức. Hệ thống kiểm tra xem công thức đã được yêu thích chưa và chuyển đổi trạng thái. Yêu thích được lưu trữ trong cơ sở dữ liệu Supabase liên kết với ID người dùng. AJAX được sử dụng để tương tác liền mạch mà không tải lại trang.

**UC-008: Xem Yêu thích của Tôi**  
Người dùng đã đăng nhập truy cập trang yêu thích cá nhân hóa của họ. Hệ thống truy xuất tất cả công thức được yêu thích cho người dùng từ cơ sở dữ liệu, tìm nạp chi tiết công thức đầy đủ từ API và hiển thị chúng trong danh sách. Nếu không có yêu thích nào, một thông báo thích hợp sẽ được hiển thị.

**UC-009: Tải Thêm Công thức**  
Khi xem công thức theo danh mục, người dùng có thể tải thêm công thức ngoài bộ ban đầu. Điểm cuối AJAX này tìm nạp lô công thức tiếp theo cho danh mục được chỉ định, duy trì hiệu suất bằng cách tải dữ liệu tăng dần.

**UC-010: Lấy Món Ngẫu nhiên**  
Người dùng có thể khám phá công thức mới bằng cách nhấp vào nút "Món Ngẫu nhiên". Hệ thống tìm nạp một công thức ngẫu nhiên từ API MealDB và chuyển hướng đến trang chi tiết của nó, cung cấp yếu tố bất ngờ và khám phá.

## Các Trường hợp Sử dụng Phi Chức năng

| ID | Mô tả | Yêu cầu Liên quan |
|----|--------|-------------------|
| NF-001 | Ứng dụng nên phản hồi yêu cầu của người dùng trong vòng 2 giây cho hầu hết các hoạt động, với bộ nhớ đệm được triển khai cho dữ liệu được truy cập thường xuyên. | Thời gian phản hồi, bộ nhớ đệm |
| NF-002 | Xác thực người dùng và bảo vệ dữ liệu bằng Supabase, với xử lý mật khẩu an toàn và quản lý phiên. | Bảo vệ dữ liệu, xác thực |
| NF-003 | Thiết kế đáp ứng được tối ưu hóa cho máy tính để bàn và di động, với điều hướng trực quan và thông báo lỗi rõ ràng. | Giao diện đáp ứng, trải nghiệm người dùng |
| NF-004 | Xử lý lỗi mạnh mẽ cho các lỗi API, vấn đề cơ sở dữ liệu và đầu vào không hợp lệ, với suy giảm nhẹ nhàng. | Xử lý lỗi, tính ổn định hệ thống |

### Giải thích Chi tiết

**NF-001: Hiệu suất**  
Hiệu suất rất quan trọng cho sự hài lòng của người dùng. Hệ thống triển khai bộ nhớ đệm trong bộ nhớ cho công thức và danh mục để giảm các cuộc gọi API. Thời gian phản hồi được giám sát, với hầu hết các hoạt động hoàn thành trong vòng 2 giây. Điều này bao gồm tải trang, tìm kiếm và truy xuất dữ liệu.

**NF-002: Bảo mật**  
Bảo mật đảm bảo bảo vệ dữ liệu người dùng và tính toàn vẹn hệ thống. Supabase xử lý xác thực với băm mật khẩu an toàn. Các phiên được quản lý phía máy chủ với thời gian hết hạn đúng cách. Tất cả truyền dữ liệu sử dụng HTTPS, và các hoạt động nhạy cảm yêu cầu xác thực.

**NF-003: Tính sử dụng**  
Giao diện người dùng thích ứng với các kích thước màn hình khác nhau bằng Bootstrap. Điều hướng trực quan với menu và breadcrumb rõ ràng. Thông báo lỗi thân thiện với người dùng và cung cấp hướng dẫn. Thiết kế tuân theo các nguyên tắc khả năng truy cập để sử dụng rộng hơn.

**NF-004: Độ tin cậy**  
Hệ thống xử lý lỗi một cách nhẹ nhàng mà không bị crash. Thời gian chờ API và vấn đề kết nối cơ sở dữ liệu được bắt và hiển thị thông báo thân thiện với người dùng. Đầu vào không hợp lệ được xác thực phía khách hàng và máy chủ. Ghi nhật ký giúp khắc phục sự cố.
