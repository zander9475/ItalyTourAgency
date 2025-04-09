
-- Table: Location
CREATE TABLE Location (
    ID int IDENTITY(1,1) PRIMARY KEY,
    name varchar(50)  NOT NULL,
    description varchar(max),
	status varchar(20) NOT NULL DEFAULT 'Draft',

	-- Status input validation constraint
	CONSTRAINT validate_location_status CHECK (status IN ('Draft', 'Active', 'Inactive')),

	-- Only allows a location to be active if it has a description
	CONSTRAINT chk_active_location CHECK (
		status = 'Draft' OR (
			description IS NOT NULL
		)
	)
);

-- Table: Reviews
CREATE TABLE Review (
    ID int IDENTITY(1,1) PRIMARY KEY,
    user_id int  NOT NULL,
    tour_id int  NOT NULL,
    rating int  NOT NULL,
    description varchar(max),
    review_date datetime  NOT NULL DEFAULT GETDATE(),

	-- Rating input validation constraint
	CONSTRAINT validate_review_rating CHECK (rating BETWEEN 1 AND 5)
);

-- Table: Tour
CREATE TABLE Tour (
    ID int IDENTITY(1,1) PRIMARY KEY,
    name varchar(50)  NOT NULL,
    description varchar(max),
    duration int,
    price numeric,
    capacity int,
	status varchar(20) NOT NULL DEFAULT 'Draft',

	-- Status input validation constraint
	CONSTRAINT validate_tour_status CHECK (status IN ('Draft', 'Active', 'Inactive')),

	-- Positive number input validation constraints
	CONSTRAINT chk_positive_duration CHECK (duration > 0),
	CONSTRAINT chk_positive_price CHECK (price > 0),
	CONSTRAINT chk_positive_capacity CHECK (capacity > 0),

	-- Only allows a tour to be active if all attributes are defined
	CONSTRAINT chk_active_tour CHECK (
        status = 'Draft' OR (
            description IS NOT NULL AND
            duration IS NOT NULL AND
            price IS NOT NULL AND
            capacity IS NOT NULL
        )
    )
);

-- Table: Tour_Instance
CREATE TABLE Tour_Instance (
    ID int IDENTITY(1,1) PRIMARY KEY,
    tour_id int  NOT NULL,
    start_date date  NOT NULL,
    end_date date  NOT NULL,
    booked_slots int  NOT NULL DEFAULT 0,
    status varchar(20)  NOT NULL DEFAULT 'Open',

	-- Date input validation constraint
	CONSTRAINT chk_valid_dates CHECK (start_date <= end_date),

	-- Status input validation constraint
	CONSTRAINT chk_tour_instance_status CHECK (status IN ('Open', 'Full', 'Cancelled'))
);

-- Table: Tour_Location
CREATE TABLE Tour_Location (
    tour_id int  NOT NULL,
    location_id int  NOT NULL,
    order_in_tour int,
    CONSTRAINT PK_Tour_Location PRIMARY KEY  (location_id,tour_id),

	-- Order in tour input validation
	CONSTRAINT chk_positive_order CHECK (order_in_tour > 0),
);

-- Table: User
CREATE TABLE "User" (
    ID int IDENTITY(1,1) PRIMARY KEY,
    name varchar(50)  NOT NULL,
    email varchar(100)  NOT NULL,
    phone_num varchar(20),
    address varchar(200),
);

CREATE TABLE Booking (
    ID int IDENTITY(1,1) PRIMARY KEY,
    user_id int  NOT NULL,
    tour_instance_id int  NOT NULL,
    booking_date datetime  NOT NULL DEFAULT GETDATE(),
    group_size int  NOT NULL,
    total_price numeric  NOT NULL,
    card_number varchar(50),
	payment_date datetime,
    status varchar(50)  NOT NULL DEFAULT 'Pending',
	payment_processed bit NOT NULL DEFAULT 0,

	-- Status input validation constraint
	CONSTRAINT chk_booking_status CHECK (
		status IN ('Confirmed', 'Pending', 'Cancelled')
	),

	-- Positive number input validation constraints
	CONSTRAINT chk_positive_group_size CHECK (group_size > 0),
	CONSTRAINT chk_positive_total_price CHECK (total_price > 0),

	-- Only allows confirmation if the payment is processed and card number is on file
	CONSTRAINT chk_confirmed_booking CHECK (
		(status = 'Confirmed' AND payment_processed = 1 AND card_number IS NOT NULL) OR
        (status IN ('Pending', 'Cancelled'))
    )
)

-- foreign keys
-- Reference: Booking_Tour_Instance (table: Booking)
ALTER TABLE Booking ADD CONSTRAINT Booking_Tour_Instance
    FOREIGN KEY (tour_instance_id)
    REFERENCES Tour_Instance (ID);

-- Reference: Review_Tour (table: Review)
ALTER TABLE Review ADD CONSTRAINT Review_Tour
    FOREIGN KEY (tour_id)
    REFERENCES Tour (ID);

-- Reference: Review_User (table: Review)
ALTER TABLE Review ADD CONSTRAINT Review_User
    FOREIGN KEY (user_id)
    REFERENCES "User" (ID);

-- Reference: Tour_Instance_Tour (table: Tour_Instance)
ALTER TABLE Tour_Instance ADD CONSTRAINT Tour_Instance_Tour
    FOREIGN KEY (tour_id)
    REFERENCES Tour (ID);

-- Reference: Tour_Location_Location (table: Tour_Location)
ALTER TABLE Tour_Location ADD CONSTRAINT Tour_Location_Location
    FOREIGN KEY (location_id)
    REFERENCES Location (ID);

-- Reference: Tour_Location_Tour (table: Tour_Location)
ALTER TABLE Tour_Location ADD CONSTRAINT Tour_Location_Tour
    FOREIGN KEY (tour_id)
    REFERENCES Tour (ID);

-- Reference: User_Bookings_User (table: Booking)
ALTER TABLE Booking ADD CONSTRAINT User_Bookings_User
    FOREIGN KEY (user_id)
    REFERENCES "User" (ID);


/* Moving the following constraints to application layer or creating triggers:

	-- Email input validation constraint
	CONSTRAINT chk_valid_email CHECK (email LIKE '%_@__%.__%'),

	-- Phone number input validation constraint
	CONSTRAINT chk_valid_phone CHECK (
		phone_num IS NULL OR 
		phone_num LIKE '+[0-9-]%' AND
		LEN(phone_num) BETWEEN 8 AND 20
	)

	-- Prevent bookings after tour starts
	CONSTRAINT chk_booking_before_tour CHECK (
		booking_date <= (SELECT start_date FROM Tour_Instance WHERE ID = tour_instance_id)
	)

	-- Only allowes confirmation if the tour instance is available
	CONSTRAINT chk_tour_instance_status CHECK (
		status <> 'Confirmed' OR
		(SELECT status FROM Tour_Instance WHERE ID = tour_instance_id) IN ('Open')
	)
*/