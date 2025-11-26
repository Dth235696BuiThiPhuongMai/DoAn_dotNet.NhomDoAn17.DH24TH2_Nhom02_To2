CREATE DATABASE QuanLyTuyenDuLich

ON
(
	name = 'QuanLyTuyenDuLich_data',
	filename = 'D:\DoAn\QuanLyTuyenDuLich_data.mdf',
	size = 8MB,
	maxsize = 100MB,
	filegrowth = 2MB)
LOG ON
(
	name = 'QuanLyTuyenDuLich_log',
	filename = 'D:\DoAn\QuanLyTuyenDuLich_log.ldf',
	size = 8MB,
	maxsize = 100MB,
	filegrowth = 2MB);
GO

USE QuanLyTuyenDuLich;
GO

--TAO BANG--
CREATE TABLE KHACHHANG
(
	maKH varchar(6) check ( maKH like 'KH[0-9][0-9][0-9][0-9]'),
	hoTen nvarchar(40),
	phai nvarchar(3),
	ngsinh datetime,
	sdt varchar(10) check ( sdt like '[0][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	dchi nvarchar(50),
	primary key (maKH)
);

CREATE TABLE NHANVIEN
(
	maNV varchar(6) check (maNV like ''),
	hoTen nvarchar(40),
	chucVu nvarchar(20),
	phai nvarchar(3),
	ngsinh datetime,
	sdt varchar(10) check ( sdt like '[0][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	dchi nvarchar(50),
	luong float,
	primary key (maNV)
);

CREATE TABLE TUYENDULICH
(
	ddDen nvarchar(50),
	ddDi nvarchar(50),
	maTuyen varchar(6) check (maTuyen like ''),
	primary key (maTuyen)
);

CREATE TABLE CHUYENDI
(
	ngKh datetime,
	tgKh time,
	maCD varchar(6),
	maNV varchar(6),
	maTuyen varchar(6),
	primary key (maCD)
);
--chua co khoa chinh
CREATE TABLE DATVE
(
	maVe varchar(20),
	maKH varchar(6),
	maCD varchar(6),
	ngDat datetime,
	trangThai nvarchar(13),
	thanhTien float,
	giaVe float,
	soLuong int
);

CREATE TABLE DOANHTHU
(
	maCD varchar(6),
	giaVe float,
	tongTien float,
	ngayLap datetime
);

--DROP DATABASE QuanLyTuyenDuLich