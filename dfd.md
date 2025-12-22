# Sơ đồ Luồng Dữ liệu cho Ứng dụng Web Công thức Nấu ăn

| ID | Mô tả | Yêu cầu Liên quan |
|----|--------|-------------------|
| DFD-001 | Sơ đồ ngữ cảnh hiển thị ranh giới tổng thể của hệ thống, với các thực thể bên ngoài (Người dùng, API MealDB, Cơ sở dữ liệu Supabase) tương tác với Ứng dụng Web Công thức Nấu ăn. Luồng dữ liệu bao gồm yêu cầu của người dùng, phản hồi API và hoạt động cơ sở dữ liệu. | Tích hợp hệ thống, kết nối API bên ngoài, tương tác cơ sở dữ liệu |
| DFD-002 | Sơ đồ này chi tiết về hệ thống xác thực, hiển thị luồng dữ liệu cho đăng nhập và đăng ký. Nó bao gồm đầu vào của người dùng, xác thực, xác thực Supabase và tạo phiên. | Xác thực người dùng, quản lý phiên |
| DFD-003 | Minh họa hệ thống quản lý công thức, bao gồm duyệt, tìm kiếm và xem chi tiết. Luồng dữ liệu từ yêu cầu của người dùng qua RecipeService đến API MealDB và quay lại giao diện người dùng. | Hiển thị công thức, chức năng tìm kiếm, hiển thị thông tin công thức |
| DFD-004 | Hiển thị hệ thống yêu thích, chi tiết cách người dùng thêm/xóa yêu thích và xem danh sách của họ. Luồng dữ liệu giữa giao diện người dùng, HomeController, UserService và cơ sở dữ liệu Supabase. | Cá nhân hóa người dùng, quản lý yêu thích, hiển thị yêu thích |
| DFD-005 | Mô tả hệ thống duyệt danh mục, hiển thị cách danh mục được tìm nạp và hiển thị, với điều hướng đến công thức được lọc. | Hiển thị danh mục, hiển thị công thức |
| DFD-006 | Minh họa cơ chế bộ nhớ đệm để cải thiện hiệu suất, hiển thị cách dữ liệu được truy cập thường xuyên (công thức, danh mục) được lưu vào bộ nhớ đệm bằng IMemoryCache. | Thời gian phản hồi, bộ nhớ đệm |
| DFD-007 | Phân tích chi tiết RecipeService thành luồng dữ liệu, hiển thị tương tác với API MealDB cho các hoạt động khác nhau (tìm kiếm, ngẫu nhiên, danh mục, lọc). | Hiển thị công thức, chức năng tìm kiếm, hiển thị danh mục, lựa chọn ngẫu nhiên |
| DFD-008 | Chi tiết tương tác UserService với cơ sở dữ liệu Supabase cho quản lý người dùng và hoạt động yêu thích. | Xác thực người dùng, cá nhân hóa người dùng, quản lý yêu thích |
| DFD-009 | Hiển thị luồng xử lý lỗi trong toàn bộ ứng dụng, bao gồm lỗi API, lỗi cơ sở dữ liệu và xác thực đầu vào của người dùng. | Xử lý lỗi, tính ổn định hệ thống |
| DFD-010 | Minh họa quản lý phiên và các biện pháp bảo mật, bao gồm kiểm tra xác thực và xử lý dữ liệu an toàn. | Bảo vệ dữ liệu, xác thực |

### Giải thích Chi tiết

**DFD-001: Sơ đồ Ngữ cảnh**  
Sơ đồ này cung cấp tổng quan về toàn bộ hệ thống, hiển thị cách Ứng dụng Web Công thức Nấu ăn tương tác với các thực thể bên ngoài. Người dùng gửi yêu cầu qua giao diện web, được xử lý qua các bộ điều khiển và dịch vụ. Dữ liệu chảy đến API MealDB cho thông tin công thức và đến Supabase cho dữ liệu người dùng và yêu thích. Phản hồi chảy ngược qua các kênh tương tự để hiển thị kết quả cho người dùng.

**DFD-002: DFD Cấp 0 - Quá trình Xác thực Người dùng**  
Tập trung vào luồng xác thực ở mức độ cao. Đầu vào của người dùng (biểu mẫu đăng nhập/đăng ký) được xử lý bởi AccountController, xác thực dữ liệu và tương tác với Supabase để xác thực. Xác thực thành công tạo phiên, trong khi thất bại kích hoạt phản hồi lỗi. Sơ đồ này hiển thị chuyển đổi dữ liệu từ đầu vào người dùng thành dữ liệu phiên an toàn.

**DFD-003: DFD Cấp 0 - Duyệt và Tìm kiếm Công thức**  
Chi tiết cách dữ liệu công thức chảy qua hệ thống. Yêu cầu của người dùng cho duyệt hoặc tìm kiếm được xử lý bởi HomeController, gọi RecipeService. Dịch vụ truy vấn API MealDB, xử lý phản hồi và trả về dữ liệu công thức được định dạng. Bộ nhớ đệm được hiển thị như kho trung gian để cải thiện hiệu suất cho các yêu cầu lặp lại.

**DFD-004: DFD Cấp 0 - Quản lý Yêu thích**  
Minh họa luồng dữ liệu chức năng yêu thích. Hành động yêu thích của người dùng đã xác thực (thêm/xóa/xem) được xử lý bởi HomeController và UserService. Dữ liệu được lưu trữ/truy xuất từ cơ sở dữ liệu Supabase, với kiểm tra xác thực cho phiên người dùng và sự tồn tại của công thức. Phản hồi AJAX cung cấp phản hồi ngay lập tức mà không tải lại trang đầy đủ.

**DFD-005: DFD Cấp 0 - Quản lý Danh mục**  
Hiển thị luồng dữ liệu liên quan đến danh mục. Danh mục được tìm nạp từ API MealDB qua RecipeService và lưu vào bộ nhớ đệm để hiệu suất. Lựa chọn của người dùng kích hoạt yêu cầu công thức được lọc, chứng minh cách dữ liệu danh mục ảnh hưởng đến logic hiển thị công thức.

**DFD-006: DFD Cấp 0 - Bộ nhớ đệm và Hiệu suất**  
Làm nổi bật hệ thống bộ nhớ đệm. Dữ liệu được truy cập thường xuyên (công thức, danh mục) được lưu trữ trong IMemoryCache sau các cuộc gọi API ban đầu. Các yêu cầu tiếp theo kiểm tra bộ nhớ đệm trước, giảm tải API và cải thiện thời gian phản hồi. Cơ chế hết hạn và làm mới bộ nhớ đệm được chỉ ra.

**DFD-007: DFD Cấp 1 - Tương tác Dịch vụ Công thức**  
Phân tích chi tiết hoạt động RecipeService. Hiển thị tương tác API chi tiết cho các điểm cuối khác nhau (tìm kiếm, ngẫu nhiên, danh mục, lọc). Phân tích dữ liệu, xử lý lỗi và định dạng phản hồi được mô tả, bao gồm cách phản hồi API được chuyển đổi thành mô hình ứng dụng.

**DFD-008: DFD Cấp 1 - Hoạt động Cơ sở dữ liệu Dịch vụ Người dùng**  
Chi tiết tương tác cơ sở dữ liệu UserService. Hiển thị hoạt động CRUD cho người dùng và yêu thích trong Supabase. Xây dựng truy vấn, thực thi và xử lý kết quả được minh họa, bao gồm xử lý lỗi cho vấn đề kết nối cơ sở dữ liệu.

**DFD-009: DFD Cấp 1 - Xử lý Lỗi và Ghi nhật ký**  
Thể hiện luồng lỗi trong toàn bộ hệ thống. Lỗi API, lỗi cơ sở dữ liệu và vấn đề xác thực được bắt ở các điểm khác nhau (bộ điều khiển, dịch vụ). Dữ liệu lỗi được ghi nhật ký và thông báo thân thiện với người dùng được tạo, với suy giảm nhẹ nhàng để ngăn hệ thống bị crash.

**DFD-010: DFD Cấp 1 - Quản lý Phiên và Bảo mật**  
Minh họa luồng dữ liệu liên quan đến bảo mật. Tạo/xác thực phiên, kiểm tra xác thực và truyền dữ liệu an toàn được hiển thị. Tích hợp xác thực Supabase và sử dụng HTTPS được làm nổi bật, với các biện pháp bảo vệ dữ liệu cho các hoạt động nhạy cảm.
