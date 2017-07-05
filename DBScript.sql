--README
--v0.1: Remove SEQ of tblRole
--		Decomposed tblBook into tblTitle and TblBook
--		Change relationship of tblTitle and tblCategory into M-M (Many-Many)
--
--v0.2:	Add tblBookStatus ref to tblBook 
--		Add col 'ISBN' in tblTitle
--		Add tblFineType ref to tblFineActivity

--CREATE DATABASE OnlineLibrary
--USE OnlineLibrary

--Phân quyền cho người dùng phần mềm này
CREATE TABLE Role
(
	ID char(3) primary key, --VD: (LIB: librarian, MEM: member, etc,...)
	Name nvarchar(50)
)


CREATE TABLE Title
(
	SEQ int identity(1, 1) primary key,
	Name nvarchar(100),
	Publisher nvarchar(50),
	PublishDate date,
	ISBN char(13), --add in v0.1
	AvailableQuan int --Available quantity
)


--Phân loại sách
CREATE TABLE Category
(
	SEQ int identity(1, 1) primary key,
	ID varchar(10) unique, 
	Name nvarchar(30) --VD: sách kỹ năng mềm, sách kỹ thuật, etc,...
)


--Phân loại ngày trả sách tùy theo loại sách
CREATE TABLE Return_Type
(
	SEQ int identity(1, 1) primary key,
	NoOfDay int
)

--Thông tin về tình trạng sách (Đã cho mượn, Đang trong kho, Đã chuyển đi, Đã mất Ref: library.uncw.edu/faq/what_does_status_library_catalog_mean
CREATE TABLE Book_Status
(
	StatusID int primary key,
	StatusName varchar(50)
)

--Thông tin về sách
CREATE TABLE Book
(
	SEQ int identity(1, 1) primary key,
	ID varchar(10) unique,
	Title int CONSTRAINT FK_B_T REFERENCES Title(SEQ),
	Author nvarchar(50),
	ReturnType int CONSTRAINT FK_B_RT REFERENCES Return_Type(SEQ),
	ImportDate date NOT NULL,
	StatusID int CONSTRAINT FK_B_BST REFERENCES Book_Status(StatusID)
)

--Thông tin category tương ứng book
CREATE TABLE Book_Category_Detail
(
	SEQ int identity(1, 1) primary key,
	BookID int CONSTRAINT FK_BCD_B REFERENCES Book(SEQ),
	CategoryID int CONSTRAINT TK_C_B REFERENCES Category(SEQ)
)

--Không dùng 'User' được vì là keyword của SQLServer
CREATE TABLE Member 
(
	SEQ int identity(1, 1) primary key,
	ID char(8) unique NOT NULL,
	LastName nvarchar (20),
	FirstName nvarchar(50),
	Sex char(1) check (Sex in ('L', 'G', 'B', 'T', 'M', 'F')),
	DOB date NULL,
	Email nvarchar(50) NULL,
	Phone char(11) NULL,
	Address nvarchar(100) NULL,
	RID char(3) CONSTRAINT FK_M_R FOREIGN KEY REFERENCES Role(ID)
)

--Table ghi lại lịch sử những lần mượn/trả sách của member
--Bảng này không biết được member nào trả trễ hạn hay đúng hạn, phải
--kết hợp với bảng dưới
CREATE TABLE Activity_History
(
	SEQ int identity(1, 1) primary key,
	MID char (8) CONSTRAINT FK_AH_M REFERENCES Member(ID),
	BID varchar(10) CONSTRAINT FK_AH_B REFERENCES Book(ID),
	BorrowDate date,--ngày mượn sách
	ReturnDate date,--ngày trả sách (sẽ update khi member đến trả)
)


--Ghi lại các loại lỗi có thể bị phạt
CREATE TABLE Fine_Type
(
	FineTypeID int primary key,
	FineTypeName varchar(50)
)


--Ghi lại những member trả sách trễ hạn
--Kết hợp với bảng hoạt động ở trên, VD: member nào có mặt trong bảng
--trên và cả ở bảng này nghĩa là member đó trả trễ sách (tìm dựa trên
--activity ID)
CREATE TABLE Fine_History
(
	SEQ int identity(1, 1) primary key,
	ASEQ int unique CONSTRAINT FK_LR_AH REFERENCES Activity_History(SEQ),
	FineTypeID int CONSTRAINT FK_FH_FT REFERENCES Fine_Type(FineTypeID), --add in v0.1 
	FineAmount float,--số tiền phạt
	--DaysLate int,--số ngày trễ 
	--(Không cần thiết do bảng này đã tổng
	--quát hóa cho nhiều loại phí phạt: mất sách, rách sách,
	--trả trễ)
)

--Ghi lại danh sách yêu thích của member
CREATE TABLE Favorite_List
(
	SEQ int identity(1, 1) primary key,
	BID int CONSTRAINT FK_FL_B REFERENCES Book(SEQ),
	MID int CONSTRAINT FK_FL_M REFERENCES Member(SEQ)
)




