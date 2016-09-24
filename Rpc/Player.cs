﻿#region using directives

using System.Threading.Tasks;
using Google.Protobuf;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using System.Collections.Generic;

#endregion

namespace PokemonGo.RocketAPI.Rpc
{
    public class Player : BaseRpc
    {
        public List<BadgeType> NewlyAwardedBadgeTypes = new List<BadgeType>();
        public List<BadgeType> EquippedBadgeTypes = new List<BadgeType>();
        public List<EquippedBadge> EquippedBadges = new List<EquippedBadge>();

        public Player(Client client) : base(client)
        {
        }

        public async Task<PlayerUpdateResponse> UpdatePlayerLocation(double latitude, double longitude, double altitude, float speed)
        {
            SetCoordinates(latitude, longitude, altitude);
            SetSpeed(speed);

            var message = new PlayerUpdateMessage
            {
                Latitude = Client.CurrentLatitude,
                Longitude = Client.CurrentLongitude
            };

            var updatePlayerLocationRequestEnvelope = GetRequestBuilder().GetRequestEnvelope(new Request[] {
                new Request
                {
                    RequestType = RequestType.PlayerUpdate,
                    RequestMessage = message.ToByteString()
                }
            });

            return await PostProtoPayload<Request, PlayerUpdateResponse>(updatePlayerLocationRequestEnvelope);
        }

        internal void SetCoordinates(double lat, double lng, double altitude)
        {
            Client.CurrentLatitude = lat;
            Client.CurrentLongitude = lng;
            Client.CurrentAltitude = altitude;
        }

        internal void SetSpeed(float speed)
        {
            Client.CurrentSpeed = speed;
        }

        public async Task<GetPlayerResponse> GetPlayer()
        {
            return await PostProtoPayload<Request, GetPlayerResponse>(RequestType.GetPlayer, new GetPlayerMessage());
        }

        public async Task<GetPlayerProfileResponse> GetPlayerProfile(string playerName)
        {
            return
                await
                    PostProtoPayload<Request, GetPlayerProfileResponse>(RequestType.GetPlayerProfile,
                        new GetPlayerProfileMessage
                        {
                            PlayerName = playerName
                        });
        }

        public async Task<CheckAwardedBadgesResponse> GetNewlyAwardedBadges()
        {
            CheckAwardedBadgesResponse checkAwardedBadgesResponse = 
                await
                    PostProtoPayload<Request, CheckAwardedBadgesResponse>(RequestType.CheckAwardedBadges,
                        new CheckAwardedBadgesMessage());

            foreach (BadgeType badgeType in checkAwardedBadgesResponse.AwardedBadges)
            {
                if (!Client.Player.NewlyAwardedBadgeTypes.Contains(badgeType))
                    Client.Player.NewlyAwardedBadgeTypes.Add(badgeType);
            }
            
            return checkAwardedBadgesResponse;
        }

        public async Task<CollectDailyBonusResponse> CollectDailyBonus()
        {
            return
                await
                    PostProtoPayload<Request, CollectDailyBonusResponse>(RequestType.CollectDailyBonus,
                        new CollectDailyBonusMessage());
        }

        public async Task<CollectDailyDefenderBonusResponse> CollectDailyDefenderBonus()
        {
            return
                await
                    PostProtoPayload<Request, CollectDailyDefenderBonusResponse>(RequestType.CollectDailyDefenderBonus,
                        new CollectDailyDefenderBonusMessage());
        }

        public async Task<EquipBadgeResponse> EquipBadge(BadgeType type)
        {
            if (EquippedBadgeTypes.Contains(type))
                return new EquipBadgeResponse();

            EquipBadgeResponse equipBadgeResponse =
                await
                    PostProtoPayload<Request, EquipBadgeResponse>(RequestType.EquipBadge,
                        new EquipBadgeMessage {BadgeType = type});

            EquippedBadge badge = equipBadgeResponse.Equipped;
            if (badge != null && !EquippedBadges.Contains(badge))
            {
                EquippedBadgeTypes.Add(type);
                EquippedBadges.Add(badge);
            }

            return equipBadgeResponse;
        }

        public async Task<LevelUpRewardsResponse> GetLevelUpRewards(int level)
        {
            return
                await
                    PostProtoPayload<Request, LevelUpRewardsResponse>(RequestType.LevelUpRewards,
                        new LevelUpRewardsMessage
                        {
                            Level = level
                        });
        }

        public async Task<SetAvatarResponse> SetAvatar(PlayerAvatar playerAvatar)
        {
            return await PostProtoPayload<Request, SetAvatarResponse>(RequestType.SetAvatar, new SetAvatarMessage
            {
                PlayerAvatar = playerAvatar
            });
        }

        public async Task<SetContactSettingsResponse> SetContactSetting(ContactSettings contactSettings)
        {
            return
                await
                    PostProtoPayload<Request, SetContactSettingsResponse>(RequestType.SetContactSettings,
                        new SetContactSettingsMessage
                        {
                            ContactSettings = contactSettings
                        });
        }

        public async Task<SetPlayerTeamResponse> SetPlayerTeam(TeamColor teamColor)
        {
            return
                await
                    PostProtoPayload<Request, SetPlayerTeamResponse>(RequestType.SetPlayerTeam, new SetPlayerTeamMessage
                    {
                        Team = teamColor
                    });
        }
    }
}