DATABASE:

-- Table structure for bank_accounts

DROP TABLE IF EXISTS bank_accounts; CREATE TABLE bank_accounts ( AccountID varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, Balance int(16) NOT NULL DEFAULT 0, OwnerID int(10) NOT NULL, AccountEditors longtext CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, AccountName varchar(60) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT 'Main', PRIMARY KEY (AccountID) USING BTREE ) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- Table structure for player_tickets

DROP TABLE IF EXISTS player_tickets; CREATE TABLE player_tickets ( reason longtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL, amount int(10) NULL DEFAULT 0, issuing_officer varchar(140) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, game_character_id int(6) NULL DEFAULT NULL ) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- Table structure for property_data

DROP TABLE IF EXISTS property_data; CREATE TABLE property_data ( PropertyID varchar(6) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, OwnerID int(6) NOT NULL DEFAULT -1, Address varchar(75) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, StorageLocation varchar(150) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, StorageInventory longtext CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, StorageSize int(6) NOT NULL DEFAULT 200, GarageLocation varchar(150) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, GarageMaxVehicles int(3) NOT NULL DEFAULT 2, EntranceLocation varchar(150) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, InteriorID int(3) NOT NULL DEFAULT 0, PRIMARY KEY (PropertyID) USING BTREE, INDEX OwnerID(OwnerID) USING BTREE, INDEX Address(Address) USING BTREE, INDEX InteriorID(InteriorID) USING BTREE ) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- Table structure for property_finance

DROP TABLE IF EXISTS property_finance; CREATE TABLE property_finance ( PropertyID varchar(6) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, PropertyPrice int(100) NOT NULL DEFAULT 0, TotalAmountPaid int(100) NOT NULL DEFAULT 0, TotalMissedPayments int(2) NOT NULL DEFAULT 0, ConsecutiveMissedPayments int(2) NOT NULL DEFAULT 0, LastPayment datetime(0) NOT NULL DEFAULT current_timestamp(0), CurrentInstallmentPayed int(11) NOT NULL DEFAULT 0, PRIMARY KEY (PropertyID) USING BTREE ) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

CREATE TABLE property_tenants ( PropertyID varchar(6) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL, TenantCharacterID int(8) NOT NULL, AccessType varchar(16) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT 'guest', PRIMARY KEY (PropertyID, TenantCharacterID) USING BTREE ) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- Table structure for server_logs

DROP TABLE IF EXISTS server_logs; CREATE TABLE server_logs ( log_id int(11) NOT NULL AUTO_INCREMENT, log_timestamp timestamp(0) NOT NULL DEFAULT current_timestamp(0), logger_username varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL, logger_identifier varchar(220) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL, log_type varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL, log_text longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL, PRIMARY KEY (log_id) USING BTREE, INDEX logger_username(logger_username) USING BTREE, INDEX logger_identifier(logger_identifier) USING BTREE, INDEX log_type(log_type) USING BTREE ) ENGINE = InnoDB AUTO_INCREMENT = 73265 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;