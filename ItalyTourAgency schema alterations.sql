-- Add authentication columns
ALTER TABLE "User"
ADD 
    password_hash varchar(255) NOT NULL,
    password_salt varchar(255) NOT NULL,
    is_locked bit NOT NULL DEFAULT 0,
    failed_login_attempts int NOT NULL DEFAULT 0,
    last_login datetime,
    account_created datetime NOT NULL DEFAULT GETDATE();

-- Ensure email is unique for login
ALTER TABLE "User"
ADD CONSTRAINT UQ_User_Email UNIQUE (email);

-- Create User_Role table (supports multiple roles per user)
CREATE TABLE User_Role (
    user_id int PRIMARY KEY,
    role_name varchar(50) NOT NULL CHECK (role_name IN ('Admin', 'Customer')),
    CONSTRAINT FK_User_Role_User FOREIGN KEY (user_id) REFERENCES "User"(ID)
);