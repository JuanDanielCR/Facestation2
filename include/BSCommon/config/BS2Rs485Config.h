/**
 *  RS485 Config
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

#ifndef __BS2_RS485_CONFIG_H__
#define __BS2_RS485_CONFIG_H__

#include "../BS2Types.h"
#include "../data/BS2Rs485Channel.h"

/**
 *	Constants for BS2Rs485Config
 */
enum {
	BS2_RS485_MAX_CHANNELS	= 4,
};

/**
 *	BS2_RS485_MODE
 */
enum {
	BS2_RS485_MODE_DISABLED		= 0,
	BS2_RS485_MODE_MASTER		= 1,
	BS2_RS485_MODE_SLAVE		= 2,
	BS2_RS485_MODE_STANDALONE	= 3,

	BS2_RS485_MODE_DEFAULT		= BS2_RS485_MODE_STANDALONE,
};

typedef uint8_t BS2_RS485_MODE;

/**
 *	BS2Rs485Config
 */
typedef struct {
	BS2_RS485_MODE mode;		///< 1 byte
	uint8_t numOfChannels;		///< 1 byte
	uint8_t reserved[2];		///< 2 bytes (packing)

	uint8_t reserved1[32];		///< 32 bytes (reserved)

	BS2Rs485Channel channels[BS2_RS485_MAX_CHANNELS]; 	///< 72 bytes
} BS2Rs485Config;

#endif	// __BS2_RS485_CONFIG_H__

