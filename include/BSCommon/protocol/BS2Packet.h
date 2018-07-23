/**
 *  Packet between Devices and BioStar Server
 *
 *  @author jylee@suprema.co.kr
 *  @see
 */

/*
 *  Copyright (c) 2014 Suprema Co., Ltd. All Rights Reserved.
 *
 *  This software is the confidential and proprietary information of
 *  Suprema Co., Ltd. ("Confidential Information").  You shall not
 *  disclose such Confidential Information and shall use it only in
 *  accordance with the terms of the license agreement you entered into
 *  with Suprema.
 */

#ifndef __BS2_PAKCET_H__
#define __BS2_PAKCET_H__

#include "../BS2Types.h"

/**
 *	Packet field size
 */
enum {
	BS2_PACKET_START_CODE_LEN	= 4,
	BS2_PACKET_DEVICE_ID_LEN	= 4,
	BS2_PACKET_FLAG_LEN			= 1,
	BS2_PACKET_SEQ_LEN			= 2,
	BS2_PACKET_VERSION_LEN		= 1,
	BS2_PACKET_INDEX_LEN		= 2,
	BS2_PACKET_TOTAL_LEN		= 2,
	BS2_PACKET_PAYLOAD_SIZE_LEN	= 4,
	BS2_PACKET_COMMAND_LEN		= 2,
	BS2_PACKET_OPTION_LEN		= 2,
	BS2_PACKET_CODE_LEN			= 2,
	BS2_PACKET_PARAM1_LEN		= 4,
	BS2_PACKET_PARAM2_LEN		= 4,
	BS2_PACKET_CHECKSUM_LEN		= 4,

	BS2_PACKET_HEADER_LEN		= 36,
	BS2_UNENCRYPTED_HEADER_LEN	= 20,
	BS2_SINGLE_PACKET_LEN		= BS2_PACKET_HEADER_LEN,
};

/**
 *	Packet field position
 */
enum {
	BS2_PACKET_START_CODE_POS	= 0,
	BS2_PACKET_CHECKSUM_POS		= 4,
	BS2_PACKET_DEVICE_ID_POS	= 8,
	BS2_PACKET_FLAG_POS			= 12,
	BS2_PACKET_VERSION_POS		= 13,
	BS2_PACKET_SEQ_POS			= 14,
	BS2_PACKET_INDEX_POS		= 16,
	BS2_PACKET_TOTAL_POS	    = 18,
	BS2_PACKET_PAYLOAD_SIZE_POS	= 20,
	BS2_PACKET_OPTION_POS		= 24,
	BS2_PACKET_CODE_POS			= 24,
	BS2_PACKET_COMMAND_POS		= 26,
	BS2_PACKET_PARAM1_POS		= 28,
	BS2_PACKET_PARAM2_POS		= 32,
};

/**
 *	Constants
 */
#define BS2_PACKET_START_CODE	"{{<<"

/**
 *	BS2_PACKET_FLAG
 */
enum {
	BS2_PACKET_FLAG_ENCRYPTED	= 0x01,		///< 0 = no encryption, 1 = use encryption
	BS2_PACKET_FLAG_RESPONSE	= 0x02,		///< 0 = request, 1 = response
	BS2_PACKET_FLAG_CONTINUE	= 0x04,		///< Packet will be continuous
};

typedef uint8_t BS2_PACKET_FLAG;

/**
 *	UDP Packet constants
 */
enum {
	BS2_UDP_PACKET_SIZE			= 1024,
	BS2_UDP_MAX_PAYLOAD			= (BS2_UDP_PACKET_SIZE - BS2_PACKET_HEADER_LEN),
	BS2_UDP_BROADCAST_ID		= 0,
};

/**
 *	BS2_PACKET_CMD
 */
enum {
	BS2_CMD_NOP							= 0x0000,	///< No-operation packet: use connection only
	BS2_CMD_HANDSHAKE					= 0x0010,	///< Handshake (Not defined yet)
	BS2_CMD_KEEP_ALIVE					= 0x0011,	///< Send keep alive

	// Device Configuration
	/* set config
	 * request packet (from sdk to bscore)
	 *     option                   : Not used.
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : config payload
	 *         +--------------+-------------+------+-------------+------+-----+
	 *         | main-payload | sub-payload | data | sub-payload | data | ... |
	 *         +--------------+-------------+------+-------------+------+-----+
	 *
	 *         Ex)
	 *         +-----------------------------------------------+------------------------+--------------+--------------------+-------------+
	 *         | (BS2_CONFIG_MASK_SYSTEM | BS2_CONFIG_MASK_IP) | BS2_CONFIG_MASK_SYSTEM | SystemConfig | BS2_CONFIG_MASK_IP | BS2IpConfig |
	 *         +-----------------------------------------------+------------------------+--------------+--------------------+-------------+
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_ACK        : An acknowledgment result as it receives the data.
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : Not used.
	 */
	BS2_CMD_SET_CONFIG					= 0x0100,

	/* get config
	 * request packet (from sdk to bscore)
	 *     option                   : Not used.
	 *     param1                   : Not used.
	 *     param2                   : config mask.
	 *     payload                  : Not used.
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload
	 *         +--------------+-------------+------+-------------+------+-----+
	 *         | main-payload | sub-payload | data | sub-payload | data | ... |
	 *         +--------------+-------------+------+-------------+------+-----+
	 *
	 *         Ex)
	 *         +-----------------------------------------------+------------------------+--------------+--------------------+-------------+
	 *         | (BS2_CONFIG_MASK_SYSTEM | BS2_CONFIG_MASK_IP) | BS2_CONFIG_MASK_SYSTEM | SystemConfig | BS2_CONFIG_MASK_IP | BS2IpConfig |
	 *         +-----------------------------------------------+------------------------+--------------+--------------------+-------------+
	 */
	BS2_CMD_GET_CONFIG					= 0x0101,
	BS2_CMD_RESET_CONFIG				= 0x0102,	///< reset config
	BS2_CMD_GET_DEFAULT_CONFIG			= 0x0103,	///< get default configuration

	BS2_CMD_GET_INFO					= 0x0120,	///< get device ID and type
	BS2_CMD_UPDATE_FIRMWARE				= 0x0123,	///< Update firmware
	BS2_CMD_FACTORY_RESET				= 0x0124,	///< Reset to factory default setting
	BS2_CMD_RESET_DATABASE				= 0x0125,	///< Delete all database
	BS2_CMD_REBOOT_DEVICE				= 0x0126,	///< Reboot device
	BS2_CMD_SET_TIME					= 0x0127,	///< set device time
	BS2_CMD_GET_TIME					= 0x0128,	///< get device time
	BS2_CMD_RS485_DISCOVER				= 0x0129,	///< Discover slave devices
	BS2_CMD_RS485_SET_SLAVE				= 0x012A,	///< Set slave devices
	BS2_CMD_UPDATE_RESOURCE				= 0x0130,	///< Update Language Resource
	BS2_CMD_LOCK_DEVICE					= 0x0131,	///< Lock device UI
	BS2_CMD_UNLOCK_DEVICE					= 0x0132,	///< Unlock device UI
	BS2_CMD_UPDATE_FIRMWARE_EX			= 0x0133,	///< Update firmware with ack
	BS2_CMD_UPDATE_RESOURCE_EX			= 0x0134,	///< Update Language Resource with ack

	BS2_CMD_RUN_ACTION					= 0x0140,	///< Run action on device

	BS2_CMD_SCAN_CARD					= 0x0180,	///< Scan card information

	/* Write smart card data to card
	 * request packet (from sdk to bscore)
	 *     option                   : Not used.
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : Smart card data (BS2SmartCardData)
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : Not used.
	 */
	BS2_CMD_WRITE_CARD					= 0x0183,

	/* Erase(Format) smart card
	 * request packet (from sdk to bscore)
	 *     option                   : Not used.
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : Not used.
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : Not used.
	 */
	BS2_CMD_ERASE_CARD					= 0x0184,

#if 0
	/* Read smart card data from card
	 * request packet (from sdk to bscore)
	 *     option                   : Not used.
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : Not used.
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : Smart card data (BS2SmartCardData)
	 */
	BS2_CMD_READ_CARD					  = 0x0183,
#endif
	BS2_CMD_SCAN_FINGERPRINT			= 0x0190,	///< Scan fingerprint
	BS2_CMD_VERIFY_FINGERPRINT			= 0x0191,	///< Verify fingerprint on device
	BS2_CMD_GET_FINGERPRINT_IMAGE		= 0x0192,	///< Get last raw-image (not used)
	BS2_CMD_SCAN_FACE					= 0x01A0,	///< Scan face

	// Event
	BS2_CMD_GET_LOG						= 0x0200,	///< read log
	BS2_CMD_CLEAR_LOG					= 0x0201,	///< clear log
//	BS2_CMD_READ_LOG_CACHE				= 0x0202,	///< read log cache (not used)
//	BS2_CMD_CLEAR_LOG_CACHE				= 0x0203,	///< clear log cache (not used)
	BS2_CMD_START_LOG_MON				= 0x0210,	///< start log monitoring
	BS2_CMD_STOP_LOG_MON				= 0x0221,	///< stop log monitoring

	/* read extended log
	 * request packet (from sdk to bscore)
	 *     option                   : event mask
	 *     param1                   : Event id of the lower bound to search for in the range.
	 *     param2                   : Maximum count of events to receive.
	 *     payload                  : EventLogFilter if it is needed.
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : count number of events
	 *     payload                  : list of event
	 *         +-------------------+------------+------+------------+------+-----+
	 *         | BS2EventExtHeader | sub-header | data | sub-header | data | ... |
	 *         +-------------------+------------+------+------------+------+-----+
	 *
	 *         Ex)
	 *         +-------------------+---------------------+-----------------+---------------------+-------------+------------------------+------+--------------------------+------+
	 *         | BS2EventExtHeader | BS2_EVENT_MASK_INFO | BS2EventExtInfo | BS2_EVENT_MASK_USER | BS2_USER_ID | BS2_EVENT_MASK_TNA_KEY | data | BS2_EVENT_MASK_RAW_IMAGE | data |
	 *         +-------------------+---------------------+-----------------+---------------------+-------------+------------------------+------+--------------------------+------+
	 */
	BS2_CMD_GET_LOG_EXT					= 0x0240,

	/* enroll users
	 * request packet (from sdk to bscore)
	 *     option                   : True to overwrite user, false otherwise
	 *     param1                   : True to check duplicate user in the device, false otherwise
	 *     param2                   : Not used.
	 *     payload                  : list of user
	 *         +--------------+-------------+------+-------------+------+-----+
	 *         | main-payload | sub-payload | data | sub-payload | data | ... |
	 *         +--------------+-------------+------+-------------+------+-----+
	 *
	 *         Ex)
	 *         +--------------------------------------------+--------------------+---------+---------------------+--------------+
	 *         | (BS2_USER_MASK_DATA | BS2_USER_MASK_PHOTO) | BS2_USER_MASK_DATA | BS2User | BS2_USER_MASK_PHOTO | BS2UserPhoto |
	 *         +--------------------------------------------+--------------------+---------+---------------------+--------------+
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_ACK        : This operation is not yet completed.
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 *     payload                  : Not used.
	 */
	BS2_CMD_SET_USER					= 0x0300,

	/* get user
	 * request packet (from sdk to bscore)
	 *     option                   : user mask.
	 *     param1                   : If payload size is 0, you can specify LIMIT and OFFSET to get just a portion of the users.
	 *                                LIMIT is computed by (param1 & 0xFFFF) and OFFSET is computed by ((param1 >> 16) & 0xFFFF)*LIMIT.
	 *                                Please note that if the limit is 0, then all of users will be sent to sdk.
	 *     param2                   : Not used.
	 *     payload                  : list of user id/name entry.
	 *         +---------+------+------+-----+
	 *         | payload | data | data | ... |
	 *         +---------+------+------+-----+
	 *
	 *         Ex)
	 *         +-----------------------+-------------+-------------+-----+
	 *         | BS2_USER_MASK_ID_ONLY | BS2_USER_ID | BS2_USER_ID | ... |
	 *         +-----------------------+-------------+-------------+-----+
	 *
	 *         +--------------------+---------------+---------------+-----+
	 *         | BS2_USER_MASK_NAME | BS2_USER_NAME | BS2_USER_NAME | ... |
	 *         +--------------------+---------------+---------------+-----+
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_ACK        : An acknowledgment result as it receives the data.
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : User count of the current packet.
	 *     param2                   : Total number of user.
	 *     payload
	 *         +--------------+-------------+------+-------------+------+-----+
	 *         | main-payload | sub-payload | data | sub-payload | data | ... |
	 *         +--------------+-------------+------+-------------+------+-----+
	 *
	 *         Ex)
	 *         +--------------------------------------------+--------------------+---------+---------------------+--------------+
	 *         | (BS2_USER_MASK_DATA | BS2_USER_MASK_PHOTO) | BS2_USER_MASK_DATA | BS2User | BS2_USER_MASK_PHOTO | BS2UserPhoto |
	 *         +--------------------------------------------+--------------------+---------+---------------------+--------------+
	 */
	BS2_CMD_GET_USER					= 0x0301,

	/* delete user
	 * request packet (from sdk to bscore)
	 *     option                   : Not used.
	 *     param1                   : If payload size is 0, you can specify LIMIT and OFFSET to delete just a portion of the users.
	 *                                LIMIT is computed by (param1 & 0xFFFF) and OFFSET is computed by ((param1 >> 16) & 0xFFFF)*LIMIT.
	 *                                Please note that if the limit is 0, then all of users will be deleted.
	 *     param2                   : Not used.
	 *     payload                  : list of user id/name entry.
	 *         +--------------+------+------+-----+
	 *         | payload      | data | data | ... |
	 *         +--------------+------+------+-----+
	 *
	 *         Ex)
	 *         +-----------------------+-------------+-------------+-----+
	 *         | BS2_USER_MASK_ID_ONLY | BS2_USER_ID | BS2_USER_ID | ... |
	 *         +-----------------------+-------------+-------------+-----+
	 *
	 *         +--------------------+---------------+---------------+-----+
	 *         | BS2_USER_MASK_NAME | BS2_USER_NAME | BS2_USER_NAME | ... |
	 *         +--------------------+---------------+---------------+-----+
	 *
	 * response packet (from bscore to sdk)
	 *     option
	 *         BS2_PROTO_ACK        : This operation is not yet completed.
	 *         BS2_PROTO_SUCCESS    : This operation is successfully executed.
	 *         Error code           : The corresponding error code (errno < 0 ? 0x10000 + errno : BS2_PROTO_SUCCESS)
	 *     param1                   : Not used.
	 *     param2                   : Not used.
	 */
	BS2_CMD_DEL_USER					= 0x0302,	///< delete user

	// Access Group
	BS2_CMD_SET_ACCESS_GROUP			= 0x0400,
	BS2_CMD_GET_ACCESS_GROUP			= 0x0401,
	BS2_CMD_DEL_ACCESS_GROUP			= 0x0402,

	// Access Group Members
	BS2_CMD_SET_ACCESS_GROUP_MEMBERS	= 0x0410,
	BS2_CMD_GET_ACCESS_GROUP_MEMBERS	= 0x0411,

	// Access Level
	BS2_CMD_SET_ACCESS_LEVEL			= 0x0420,
	BS2_CMD_GET_ACCESS_LEVEL			= 0x0421,
	BS2_CMD_DEL_ACCESS_LEVEL			= 0x0422,

	// Schedule
	BS2_CMD_SET_SCHEDULE				= 0x0430,
	BS2_CMD_GET_SCHEDULE				= 0x0431,
	BS2_CMD_DEL_SCHEDULE				= 0x0432,

	// Holiday Group
	BS2_CMD_SET_HOLIDAY_GROUP			= 0x0440,
	BS2_CMD_GET_HOLIDAY_GROUP			= 0x0441,
	BS2_CMD_DEL_HOLIDAY_GROUP			= 0x0442,

	// Blacklist
	BS2_CMD_SET_BLACK_LIST				= 0x0450,
	BS2_CMD_GET_BLACK_LIST				= 0x0451,
	BS2_CMD_DEL_BLACK_LIST				= 0x0452,

	// Door
	BS2_CMD_SET_DOOR					= 0x0500,
	BS2_CMD_GET_DOOR					= 0x0501,

	/**
	 * param1 - count(doorID)
	 * option - confirmation [0: delete specified, 1: delete all in case of no payload, else delete unspecified.]
	 * payload - list(doorID)
	 */
	BS2_CMD_DEL_DOOR					= 0x0502,
	BS2_CMD_LOCK_DOOR					= 0x0510,
	BS2_CMD_UNLOCK_DOOR					= 0x0511,
	BS2_CMD_GET_DOOR_STATUS				= 0x0512,
	BS2_CMD_RELEASE_DOOR				= 0x0513,
	BS2_CMD_SET_DOOR_ALARM			= 0x0514,

	// Zone
	BS2_CMD_SET_APB_ZONE				= 0x0600,
	BS2_CMD_GET_APB_ZONE				= 0x0601,

	/**
	 * param1 - count(zoneID)
	 * option - confirmation [0: delete specified, 1: delete all in case of no payload, else delete unspecified.]
	 * payload - list(zoneID)
	 */
	BS2_CMD_DEL_APB_ZONE				= 0x0602,
	BS2_CMD_CLEAR_APB_ACCESS_RECORD				= 0x0603,
	BS2_CMD_SET_APB_ZONE_ALARM			= 0x0604,

	BS2_CMD_SET_TIMED_APB_ZONE			= 0x0610,
	BS2_CMD_GET_TIMED_APB_ZONE			= 0x0611,

	/**
	 * param1 - count(zoneID)
	 * option - confirmation [0: delete specified, 1: delete all in case of no payload, else delete unspecified.]
	 * payload - list(zoneID)
	 */
	BS2_CMD_DEL_TIMED_APB_ZONE			= 0x0612,
	BS2_CMD_CLEAR_TIMED_APB_ACCESS_RECORD				= 0x0613,
	BS2_CMD_SET_TIMED_APB_ZONE_ALARM			= 0x0614,

	BS2_CMD_SET_FIRE_ALARM_ZONE			= 0x0620,
	BS2_CMD_GET_FIRE_ALARM_ZONE			= 0x0621,

	/**
	 * param1 - count(zoneID)
	 * option - confirmation [0: delete specified, 1: delete all in case of no payload, else delete unspecified.]
	 * payload - list(zoneID)
	 */
	BS2_CMD_DEL_FIRE_ALARM_ZONE			= 0x0622,
	BS2_CMD_SET_FIRE_ALARM_ZONE_ALARM			= 0x0623,

	BS2_CMD_SET_SCHEDULED_LOCK_ZONE		= 0x0630,
	BS2_CMD_GET_SCHEDULED_LOCK_ZONE		= 0x0631,

	/**
	 * param1 - count(zoneID)
	 * option - confirmation [0: delete specified, 1: delete all in case of no payload, else delete unspecified.]
	 * payload - list(zoneID)
	 */
	BS2_CMD_DEL_SCHEDULED_LOCK_ZONE		= 0x0632,
	BS2_CMD_SET_SCHEDULED_LOCK_ZONE_ALARM			= 0x0633,

	/**
	 * param1: count(zoneID)
	 * param2: zoneType
	 * payload: list(zoneID)
	 */
	BS2_CMD_GET_ZONE_STATUS             = 0x0634,

	// Wiegand
	BS2_CMD_WIEGAND_DISCOVER			= 0x0700,
	BS2_CMD_ADD_WIEGAND_DEVICE			= 0x0701,
	BS2_CMD_DEL_WIEGAND_DEVICE			= 0x0702,
	BS2_CMD_GET_WIEGAND_DEVICE			= 0x0703,

	// UDP command
	BS2_CMD_UDP_DISCOVER				= 0x1000,	///< search devices using UDP
	BS2_CMD_UDP_GET_CONFIG				= 0x1001,	///< read config using UDP
	BS2_CMD_UDP_SET_CONFIG				= 0x1002,	///< write config using UDP
	BS2_CMD_UDP_RESET					= 0x1003,	///< reset device using UDP (Not defined yet)

	// Command from device-oriented
	BS2_CMD_NOTIFY_EVENT				= 0x2000,	///< Notify event to server
	BS2_CMD_NOTIFY_CONFIG				= 0x2001,	///< Notify configuration to server
	BS2_CMD_NOTIFY_INPUT				= 0x2002,	///< Notify input to server
	BS2_CMD_NOTIFY_ALARM			= 0x2003,	///< Notify alarm to server

	/**
	 * param1 - credential[0: Card, 1: ID]
	 * param2 - cardType[0: Unknown, 1: CSN, 2: Secure, 3: AOC, 0x0A: Wiegand]
	 * payload - cardData or userID
	 */
	BS2_CMD_SERVER_VERIFY				= 0x2010,	///< verify user with server

	/**
	 * param1 - templateFormat[0xFF: Unknown, 0: Suprema, 1: ISO, 2: ANSI]
	 * payload - templateData
	 */
	BS2_CMD_SERVER_IDENTIFY				= 0x2011,	///< Identify user with server

	/* check apb violation status via server
	 * request packet (from bscore to sdk)
	 *     id                       : reader ID
	 *     param1                   : Total number of user ID.
	 *     param2                   : Dual auth flag (0 : Normal auth, 1 : Dual auth)
	 *     payload                  : list of user ID.
	 *
	 * response packet (from server to bscore)
	 *     param1                   : zoon ID
	 *     param2                   : apb violation status [0: No error, 1: Soft apb violation, 2: Hard apb violation]
	 */
	BS2_CMD_CHECK_APB_VIOLATION     = 0x2012,

	// Factory command
	BS2_CMD_SET_DEVICE_ID				= 0x8000,	///< Write device id
	BS2_CMD_SET_MAC_ADDRESS				= 0x8001,
	BS2_CMD_SET_MODEL_NAME				= 0x8002,

	// Test Command (Not included on release)
	BS2_CMD_OPEN_DOOR					= 0x8010,
	BS2_CMD_CLOSE_DOOR					= 0x8011,

	BS2_CMD_AUTH_CREDENTIAL				= 0x8020,

	// SSL Command
	BS2_CMD_GET_SYSTEM_INFO				= 0x9001,
	BS2_CMD_SET_ROOT_CA   				= 0x9002,
	BS2_CMD_SSL_ENTER    				= 0x9003,
	BS2_CMD_SSL_LEAVE    				= 0x9004,
};

typedef uint16_t BS2_PACKET_CMD;

/**
 *	Packet Return Code
 */
enum {
	BS2_PROTO_SUCCESS			= 0x00,
	BS2_PROTO_ACK				= 0x01,
	BS2_PROTO_BUSY = 0x02,

	BS2_PROTO_UNSUPPORTED		= 0x100,
	BS2_PROTO_INVALID_OPTION	= 0x101,
	BS2_PROTO_INVALID_PAYLOAD	= 0x102,
	BS2_PROTO_DUPLICATED_USER	= 0x103,
	BS2_PROTO_USER_FULL			= 0x104,
	BS2_PROTO_USER_DB_ERROR		= 0x105,
	BS2_PROTO_NO_USER			= 0x106,
	BS2_PROTO_CANNOT_READ_LOG	= 0x107,
};

typedef uint32_t BS2_PACKET_START;
typedef uint16_t BS2_PACKET_SEQ;
typedef uint16_t BS2_PACKET_INDEX;
typedef uint16_t BS2_PACKET_TOTAL;
typedef uint16_t BS2_PACKET_OPTION;
typedef uint16_t BS2_PACKET_CODE;
typedef uint8_t BS2_PACKET_VER;
typedef uint32_t BS2_PACKET_PARAM;
typedef uint32_t BS2_PACKET_SIZE;
typedef uint32_t BS2_PACKET_CHECKSUM;

/**
 *	Type definition of Packet Header
 */
typedef struct {
	BS2_PACKET_START	startCode;		///< start code of packet
	BS2_PACKET_CHECKSUM	checksum;		///< checksum for packet data except first 8 bytes
	BS2_DEVICE_ID		id;				///< device id
	union {
		BS2_PACKET_FLAG	flag;			///< flag of packet: @ref BS2_PACKET_FLAG_
		struct {
			BS2_PACKET_FLAG encrypted: 1;
			BS2_PACKET_FLAG response: 1;
			BS2_PACKET_FLAG more: 1;
			BS2_PACKET_FLAG reserved: 5;
		};
	};
	BS2_PACKET_VER		version;		///< version of protocol
	BS2_PACKET_SEQ		sequence;		///< sequence of packet
	BS2_PACKET_INDEX	index;			///< index number of continuous packet
	BS2_PACKET_TOTAL	total;			///< total number of continuous packet
	BS2_PACKET_SIZE		payloadSize;	///< size of payload
	union {
		BS2_PACKET_OPTION	option;		///< will be used for request packet
		BS2_PACKET_CODE		code;		///< will be used for response packet
	};
	BS2_PACKET_CMD		command;		///< command: @ref BS2_PACKET_CMD
	BS2_PACKET_PARAM	param1;			///< first parameter	: varies for each command
	BS2_PACKET_PARAM	param2;			///< second parameter	: varies for each command
} BS2PacketHeader;

/**
 *	BS2_PAYLOAD_MASK
 */
typedef uint32_t BS2_HEADER_MASK;
typedef uint32_t BS2_HEADER_SIZE;

/**
 *	Type definition of payload header
 *
 *	Payload:
 *		+-------------+------------+------+------------+------+-----+
 *		| main-header | sub-header | data | sub-header | data | ... |
 *		+-------------+------------+------+------------+------+-----+
 *		or
 *		+----------------+------+------+------+-----+
 *		| header(mask 0) | data | data | data | ... |
 *		+----------------+------+------+------+-----+
 */
typedef struct {
	BS2_HEADER_MASK		mask;	///< Mask of payload data
	BS2_HEADER_SIZE		size;	///< Size of payload data
} BS2PayloadHeader;

#endif	// __BS2_PAKCET_H__
