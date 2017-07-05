--CREATE DATABASE OnlineLibrary
--USE OnlineLibrary


--Phân quyền cho người dùng phần mềm này
CREATE TABLE Role
(
	ID char(3) primary key, --VD: (LIB: librarian, MEM: member, etc,...)
	Name nvarchar(50)
)


--Phân loại sách
CREATE TABLE Category
(
	SEQ int identity(1, 1) primary key,
	ID varchar(10) unique, 
	Name nvarchar(30) --VD: sách kỹ năng mềm, sách kỹ thuật, etc,...
)


--Phân loại ngày trả sách tùy theo loại sách
CREATE TABLE ReturnType
(
	SEQ int identity(1, 1) primary key,
	NoOfDay int
)


--Thông tin về sách
CREATE TABLE Book
(
	SEQ int identity(1, 1) primary key,
	ID varchar(10) unique,
	Name nvarchar(50),
	Author nvarchar(50),
	ReturnType int CONSTRAINT FK_B_RT REFERENCES ReturnType(SEQ),
	ImportDate date NOT NULL,
	Quantity int
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


--Ghi lại những member trả sách trễ hạn
--Kết hợp với bảng hoạt động ở trên, VD: member nào có mặt trong bảng
--trên và cả ở bảng này nghĩa là member đó trả trễ sách (tìm dựa trên
--activity ID)
CREATE TABLE Late_Returner
(
	SEQ int identity(1, 1) primary key,
	ASEQ int unique CONSTRAINT FK_LR_AH REFERENCES Activity_History(SEQ),
	FineAmount float,--số tiền phạt
	DaysLate int,--số ngày trễ
)

--Ghi lại danh sách yêu thích của member
CREATE TABLE Favorite_List
(
	SEQ int identity(1, 1) primary key,
	BID int CONSTRAINT FK_FL_B REFERENCES Book(SEQ),
	MID int CONSTRAINT FK_FL_M REFERENCES Member(SEQ)
)


