/**
 *  Face
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

#ifndef __BS2_FACE_H__
#define __BS2_FACE_H__

#include "../BS2Types.h"

/**
 *	constants for BS2Face
 */
enum {
	BS2_FACE_TEMPLATE_SIZE	= 2048,
	BS2_TEMPLATE_PER_FACE	= 25,
};

/**
 *	BS2Face
 */
typedef struct {
	uint8_t		faceIndex;
	uint8_t		templateIndex;
	uint8_t		reserved[2];
	uint8_t		data[BS2_TEMPLATE_PER_FACE][BS2_FACE_TEMPLATE_SIZE];
} BS2Face;

#endif	// __BS2_FACE_H__
