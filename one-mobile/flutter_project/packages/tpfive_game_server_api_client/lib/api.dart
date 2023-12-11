//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

library openapi.api;

import 'dart:async';
import 'dart:convert';
import 'dart:io';

import 'package:http/http.dart';
import 'package:intl/intl.dart';
import 'package:meta/meta.dart';

part 'api_client.dart';
part 'api_helper.dart';
part 'api_exception.dart';
part 'auth/authentication.dart';
part 'auth/api_key_auth.dart';
part 'auth/oauth.dart';
part 'auth/http_basic_auth.dart';
part 'auth/http_bearer_auth.dart';

part 'api/agora_api.dart';
part 'api/aigc_api.dart';
part 'api/asset_api.dart';
part 'api/avatar_api.dart';
part 'api/decoration_api.dart';
part 'api/feed_api.dart';
part 'api/login_api.dart';
part 'api/reel_api.dart';
part 'api/room_api.dart';
part 'api/space_api.dart';

part 'model/agora_streaming_token_data.dart';
part 'model/agora_streaming_token_payload.dart';
part 'model/asset.dart';
part 'model/asset_list_response.dart';
part 'model/asset_list_response_all_of.dart';
part 'model/asset_page.dart';
part 'model/asset_page_all_of.dart';
part 'model/avatar_activate_response.dart';
part 'model/avatar_activate_response_all_of.dart';
part 'model/avatar_activate_response_data.dart';
part 'model/avatar_metadata.dart';
part 'model/avatar_metadata_list_response.dart';
part 'model/avatar_metadata_list_response_all_of.dart';
part 'model/avatar_metadata_list_response_data.dart';
part 'model/avatar_metadata_response.dart';
part 'model/avatar_metadata_response_all_of.dart';
part 'model/avatar_metadata_response_data.dart';
part 'model/avatar_model_type.dart';
part 'model/avatar_thumbnail.dart';
part 'model/base_response.dart';
part 'model/categories_enum.dart';
part 'model/category_item.dart';
part 'model/confirm_uploaded_response.dart';
part 'model/confirm_uploaded_response_all_of.dart';
part 'model/create_reel_request.dart';
part 'model/create_reel_response.dart';
part 'model/create_reel_response_all_of.dart';
part 'model/create_reel_response_data.dart';
part 'model/create_upload_request.dart';
part 'model/create_upload_request_response.dart';
part 'model/create_upload_request_response_all_of.dart';
part 'model/create_upload_request_response_data.dart';
part 'model/decoration.dart';
part 'model/decoration_category_page.dart';
part 'model/decoration_category_page_all_of.dart';
part 'model/decoration_page.dart';
part 'model/decoration_page_all_of.dart';
part 'model/feed.dart';
part 'model/feed_content.dart';
part 'model/generate_motion_request.dart';
part 'model/generate_motion_response.dart';
part 'model/generate_motion_response_all_of.dart';
part 'model/get_agora_streaming_token_response.dart';
part 'model/get_agora_streaming_token_response_all_of.dart';
part 'model/get_decoration_category_list_response.dart';
part 'model/get_decoration_category_list_response_all_of.dart';
part 'model/get_decoration_list_response.dart';
part 'model/get_decoration_list_response_all_of.dart';
part 'model/get_decoration_response.dart';
part 'model/get_decoration_response_all_of.dart';
part 'model/get_feed_list_response.dart';
part 'model/get_feed_list_response_all_of.dart';
part 'model/get_feed_list_response_all_of_data.dart';
part 'model/get_profile_response.dart';
part 'model/get_profile_response_all_of.dart';
part 'model/get_space_group_list_response.dart';
part 'model/get_space_group_list_response_all_of.dart';
part 'model/get_space_group_list_response_all_of_data.dart';
part 'model/get_space_list_response.dart';
part 'model/get_space_list_response_all_of.dart';
part 'model/get_space_list_response_all_of_data.dart';
part 'model/join_mode_enum.dart';
part 'model/list_reel_response.dart';
part 'model/list_reel_response_all_of.dart';
part 'model/list_reel_response_all_of_data.dart';
part 'model/pagination_base.dart';
part 'model/profile.dart';
part 'model/reel.dart';
part 'model/register_room_user_response.dart';
part 'model/register_room_user_response_all_of.dart';
part 'model/room.dart';
part 'model/room_user.dart';
part 'model/room_user_registry.dart';
part 'model/s3_object.dart';
part 'model/space.dart';
part 'model/space_group.dart';
part 'model/space_group_spaces_inner.dart';
part 'model/unregister_room_user_response.dart';
part 'model/upload_file.dart';


const _delimiters = {'csv': ',', 'ssv': ' ', 'tsv': '\t', 'pipes': '|'};
const _dateEpochMarker = 'epoch';
final _dateFormatter = DateFormat('yyyy-MM-dd');
final _regList = RegExp(r'^List<(.*)>$');
final _regSet = RegExp(r'^Set<(.*)>$');
final _regMap = RegExp(r'^Map<String,(.*)>$');

ApiClient defaultApiClient = ApiClient();
