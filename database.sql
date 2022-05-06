create table campus
(
    id       int auto_increment
        primary key,
    name     varchar(45)  default 'Campus Name'               not null,
    location varchar(100) default 'Default Address 29N, 2200' not null,
    ssid     varchar(45)                                      null,
    constraint campus_id_uindex
        unique (id)
)
    auto_increment = 2;

create table role
(
    id   int auto_increment
        primary key,
    name varchar(45) not null
)
    auto_increment = 5;

create table trophy
(
    id        int auto_increment
        primary key,
    name      varchar(45) default 'Default Trophy' not null,
    automatic tinyint(1)                           null,
    constraint trophy_id_uindex
        unique (id)
);

create table user
(
    id        int auto_increment
        primary key,
    email     varchar(45) not null,
    password  varchar(45) not null,
    firstName varchar(45) not null,
    lastName  varchar(45) not null,
    role_id   int         null,
    constraint user_role_id_fk
        foreign key (role_id) references role (id)
)
    auto_increment = 9;

create table student_trophies
(
    student_id int not null,
    trophy_id  int not null,
    primary key (student_id, trophy_id),
    constraint student_trophies_student_id_fk
        foreign key (student_id) references user (id),
    constraint student_trophies_trophy_id_fk
        foreign key (trophy_id) references trophy (id)
);

create index student_id
    on student_trophies (student_id);

create table subject
(
    id         int auto_increment
        primary key,
    teacher_id int         null,
    name       varchar(45) not null,
    constraint subject_id_uindex
        unique (id),
    constraint subject_name_uindex
        unique (name),
    constraint subject_teacher_id_fk
        foreign key (teacher_id) references user (id)
)
    auto_increment = 115;

create table lesson
(
    id         int auto_increment
        primary key,
    subject_id int      null,
    startTime  datetime not null,
    code       int      null,
    codeTime   datetime null,
    campus_id  int      null,
    constraint lesson_campus_id_fk
        foreign key (campus_id) references campus (id),
    constraint lesson_subject_id_fk
        foreign key (subject_id) references subject (id)
)
    auto_increment = 112;

create table attending_student
(
    student_id int not null,
    lesson_id  int not null,
    primary key (student_id, lesson_id),
    constraint attending_student_lesson_id_fk
        foreign key (lesson_id) references lesson (id),
    constraint attending_student_student_id_fk
        foreign key (student_id) references user (id)
);

create table subject_student
(
    subject_id int not null,
    student_id int not null,
    primary key (subject_id, student_id),
    constraint subject_student_student_id_fk
        foreign key (student_id) references user (id),
    constraint subject_student_subject_id_fk
        foreign key (subject_id) references subject (id)
);

create index subject_id
    on subject_student (subject_id);

create definer = oz8r3qp1wop1mbpw@`%` view studentlist as
select `c0h3ipgv5ohwo5f2`.`user`.`id` AS `id`, `c0h3ipgv5ohwo5f2`.`user`.`email` AS `email`
from `c0h3ipgv5ohwo5f2`.`user`
where (`c0h3ipgv5ohwo5f2`.`user`.`role_id` = 0);

create definer = oz8r3qp1wop1mbpw@`%` view students_attending_subjects as
select `c0h3ipgv5ohwo5f2`.`user`.`email`                        AS `email`,
       count(`c0h3ipgv5ohwo5f2`.`subject_student`.`student_id`) AS `subjects`
from (`c0h3ipgv5ohwo5f2`.`subject_student` left join `c0h3ipgv5ohwo5f2`.`user`
      on ((`c0h3ipgv5ohwo5f2`.`subject_student`.`student_id` = `c0h3ipgv5ohwo5f2`.`user`.`id`)))
group by `c0h3ipgv5ohwo5f2`.`user`.`email`
order by `subjects` desc;

create definer = oz8r3qp1wop1mbpw@`%` view teacherlist as
select `c0h3ipgv5ohwo5f2`.`user`.`id` AS `id`, `c0h3ipgv5ohwo5f2`.`user`.`email` AS `email`
from `c0h3ipgv5ohwo5f2`.`user`
where (`c0h3ipgv5ohwo5f2`.`user`.`role_id` = 1);

create
    definer = oz8r3qp1wop1mbpw@`%` function attendance_percentage(studentId int, subjectId int) returns int
begin
declare totalLessons int;
declare attendedLessons int;
declare result int;

select count(*) into totalLessons from lesson where subject_id = subjectId;
select count(*) into attendedLessons from lesson
join attending_student `as` on lesson.id = `as`.lesson_id
join user s on `as`.student_id = s.id
where subject_id = subjectId and student_id = studentId;

select floor(attendedLessons/totalLessons * 100) into result;
return result;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` procedure insertFakeData_Attending_Student()
BEGIN
    DECLARE no INT;
    SET no = 1;
    `loop`:
    LOOP
        insert into attending_student (lesson_id, student_id)
        values (floor(rand() * (100-1) + 1),floor(rand() * (100-1) + 1));
        SET no = no + 1;
        IF no = 100 THEN
            LEAVE `loop`;
        END IF;
    END LOOP `loop`;
    SELECT no;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` procedure insertFakeData_Lesson()
BEGIN
    DECLARE no INT;
    SET no = 1;
    `loop`:
    LOOP
        insert into lesson (subject_id, startTime, code, codeTime, ipAddress)
        values (floor(rand() * (100-10) + 10), now(), 3232, now(), '192.168.1.124');
        SET no = no + 1;
        IF no = 100 THEN
            LEAVE `loop`;
        END IF;
    END LOOP `loop`;
    SELECT no;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` procedure insertFakeData_Student()
BEGIN
    DECLARE no INT;
    SET no = 1;
    `loop`:
    LOOP
        insert into student (email, password)
        values (CONCAT(CONVERT(no, char), '@gmail.com'), '1234abcd');
        SET no = no + 1;
        IF no = 100 THEN
            LEAVE `loop`;
        END IF;
    END LOOP `loop`;
    SELECT no;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` procedure insertFakeData_Subject()
BEGIN
    DECLARE no INT;
    SET no = 1;
    `loop`:
    LOOP
        insert into subject (teacher_id, name)
        values (no,CONCAT('subject',CONVERT(no, char)));
        SET no = no + 1;
        IF no = 100 THEN
            LEAVE `loop`;
        END IF;
    END LOOP `loop`;
    SELECT no;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` procedure insertFakeData_Subject_Student()
BEGIN
    DECLARE no INT;
    SET no = 1;
    `loop`:
    LOOP
        insert into subject_student (subject_id, student_id)
        values (floor(rand() * (100-10) + 10),floor(rand() * (100-1) + 1));
        SET no = no + 1;
        IF no = 100 THEN
            LEAVE `loop`;
        END IF;
    END LOOP `loop`;
    SELECT no;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` procedure insertFakeData_Teacher()
BEGIN
    DECLARE no INT;
    SET no = 4;
    `loop`:
    LOOP
        insert into teacher (email, password)
        values (CONCAT(CONVERT(no, char), '@gmail.com'), '1234abcd');
        SET no = no + 1;
        IF no = 100 THEN
            LEAVE `loop`;
        END IF;
    END LOOP `loop`;
    SELECT no;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` function lesson_attendance_percentage(lessonId int) returns int
begin
declare totalStudents int;
declare attendingStudents int;
declare result int;

select count(*) into totalStudents from lesson
join subject `su` on subject_id = `su`.id
join subject_student `ss` on `ss`.subject_id = `su`.id
where lesson.id = lessonId;
select count(*) into attendingStudents from attending_student where lesson_id = lessonId;

select floor(attendingStudents/totalStudents * 100) into result;
return result;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` procedure sp_GetAttendingstudents()
BEGIN
    select student_id, lesson_id from attending_student;
END;

create
    definer = oz8r3qp1wop1mbpw@`%` function total_attendance_percentage(studentId int) returns int
begin
declare totalLessons int;
declare attendedLessons int;
declare result int;

select count(*) into totalLessons from lesson
join subject `su` on subject_id = `su`.id
join subject_student `ss` on `ss`.subject_id = `su`.id
where `ss`.student_id = studentId;
select count(*) into attendedLessons from attending_student where student_id = studentId;

select floor(attendedLessons/totalLessons * 100) into result;
return result;
END;


