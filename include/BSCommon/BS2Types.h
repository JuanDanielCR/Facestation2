/**
 *  Common type definition
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

#ifndef __BS2_TYPES_H__
#define __BS2_TYPES_H__

/**
 *	Size of data types
 *
 *	@TODO: replace platform independent code
 */

#if defined(_MSC_VER) && (_MSC_VER < 1600)
typedef __int8 int8_t;
typedef unsigned __int8 uint8_t;
typedef __int16 int16_t;
typedef unsigned __int16 uint16_t;
typedef __int32 int32_t;
typedef unsigned __int32 uint32_t;
typedef unsigned __int64 uint64_t;
typedef __int64 int64_t;
#else
#include <stdint.h>
#endif /* defined(_MSC_VER) && (_MSC_VER < 1600) */

#ifndef __cplusplus
#if defined(__STDC_VERSION__) && (__STDC_VERSION__ >= 199901L)
#include <stdbool.h>
#else
#define bool int
#define true	1
#define false	0
#endif /* defined(__STDC_VERSION__) && (__STDC_VERSION__ >= 199901L) */
#endif /* __cplusplus */

/**
 *  Define BS2_FOURCC
 */
#define BS2_FOURCC(a, b, c, d)	(((uint32_t)(a)) | ((uint32_t)(b) << 8) | ((uint32_t)(c) << 16) | ((uint32_t)(d) << 24))

enum {
	BS2_IPV4_ADDR_SIZE		= 16,
	BS2_URL_SIZE		= 256,
	BS2_USER_ID_SIZE		= 32,			///< Alpha-numeric

	BS2_USER_NAME_LEN		= 48 * 4,		///< UTF-8 Encoding
	BS2_USER_IMAGE_SIZE		= 16 * 1024,
	BS2_PIN_HASH_SIZE		= 32,
};

/**
 *	@note
 *		Use 'char array' for string which has only alpha-numeric value.
 *		If string has to be encoded with UTF-8, use 'array of uint8_t' instead.
 *
 *		Do not use 'bool' type because its size varies by compiler.
 *		Use 'BS_BOOL' instead.
 *
 *		For example:
 *				user id		- defined as 'char[32]'
 *				user name	- defined as 'uint8_t[48 * 4];
 */
typedef uint32_t	BS2_UID;
typedef uint32_t	BS2_DEVICE_ID;
typedef uint32_t	BS2_EVENT_ID;
typedef uint32_t	BS2_TICK;
typedef uint32_t	BS2_TIMESTAMP;
typedef uint32_t	BS2_DATETIME;
typedef uint32_t	BS2_VERSION;
typedef char		BS2_USER_ID[BS2_USER_ID_SIZE];
typedef char		BS2_IPV4_ADDR[BS2_IPV4_ADDR_SIZE];
typedef char		BS2_URL[BS2_URL_SIZE];
typedef uint16_t	BS2_PORT;
typedef uint32_t	BS2_CHECKSUM;
typedef uint8_t		BS2_HASH256[BS2_PIN_HASH_SIZE];
typedef bool		BS2_BOOL;
typedef BS2_UID		BS2_DOOR_ID;
typedef BS2_UID		BS2_ZONE_ID;
typedef BS2_UID		BS2_ACCESS_GROUP_ID;
typedef BS2_UID		BS2_ACCESS_LEVEL_ID;
typedef BS2_UID		BS2_SCHEDULE_ID;
typedef BS2_UID		BS2_HOLIDAY_GROUP_ID;

#define BS2_INVALID_PORT 0xFFFF

#ifdef FILE_SYSTEM_SUPPORTED
typedef uint32_t		BS2_SLOT_INDEX;
typedef uint32_t		BS2_LINK_ID;
#else
typedef uint16_t		BS2_SLOT_INDEX;
typedef uint16_t		BS2_LINK_ID;
#endif

#define BS2_INVALID_LINK_ID		0

/**
 *	Enumeration of BS2_DEVICE_TYPE
 */
enum {
	BS2_DEVICE_TYPE_BIOENTRY_PLUS		= 0x01,
	BS2_DEVICE_TYPE_BIOENTRY_W			= 0x02,
	BS2_DEVICE_TYPE_BIOLITE_NET			= 0x03,
	BS2_DEVICE_TYPE_XPASS				= 0x04,
	BS2_DEVICE_TYPE_XPASS_S2			= 0x05,
	BS2_DEVICE_TYPE_ENTRY_MAX			= 0x05,
	BS2_DEVICE_TYPE_SECURE_IO_2			= 0x06,
	BS2_DEVICE_TYPE_DOOR_MODULE_20		= 0x07,
	BS2_DEVICE_TYPE_BIOSTATION_2		= 0x08,
	BS2_DEVICE_TYPE_BIOSTATION_A2		= 0x09,
	BS2_DEVICE_TYPE_FACESTATION_2		= 0x0A,
	BS2_DEVICE_TYPE_IO_DEVICE			= 0x0B,
#if defined(BSL2_AS_BS2)
	BS2_DEVICE_TYPE_BIOSTATION_L2		= BS2_DEVICE_TYPE_BIOSTATION_2,
#else
	BS2_DEVICE_TYPE_BIOSTATION_L2		= 0x0C,
#endif
#if defined(BEW2_AS_BEW)
	BS2_DEVICE_TYPE_BIOENTRY_W2 		= BS2_DEVICE_TYPE_BIOENTRY_W,
#else
	BS2_DEVICE_TYPE_BIOENTRY_W2			= 0x0D,
#endif	
	BS2_DEVICE_TYPE_UNKNOWN				= 0xFF,
};

typedef uint16_t BS2_DEVICE_TYPE;

enum {
	BS2_CARD_MODEL_OMPW = 0,
	BS2_CARD_MODEL_OIPW,
	BS2_CARD_MODEL_OEPW,
	BS2_CARD_MODEL_OHPW,
};

typedef uint16_t BS2_CARD_MODEL;

/**
 *  BS2_SWITCH_TYPE
 */
enum {
	BS2_SWITCH_TYPE_NORMAL_OPEN = 0,
	BS2_SWITCH_TYPE_NORMAL_CLOSED,
};

typedef uint8_t BS2_SWITCH_TYPE;

enum {
	BS2_PORT_VALUE_UNKNOWN = -1,
	BS2_PORT_VALUE_OPEN = 0,
	BS2_PORT_VALUE_CLOSED,
	BS2_PORT_VALUE_SUPERVISED_SHORT = 2,
	BS2_PORT_VALUE_SUPERVISED_OPEN,
};

typedef int8_t BS2_PORT_VALUE;

#endif	// __BS2_TYPES_H__
