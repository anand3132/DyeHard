using System.Collections.Generic;
using PaintCore;
using UnityEngine;

namespace RedGaint
{

    public class TeamData
    {
        public Color TeamColor { get; private set; }
        public string TeamName { get; private set; }
        public HashSet<string> MemberNames { get; private set; } // Ensures unique member names within a team
        public uint TeamStrength { get; set; }

        public TeamData(Color teamColor, string teamName)
        {
            TeamColor = teamColor;
            TeamName = teamName;
            MemberNames = new HashSet<string>();
            TeamStrength = 0;
        }

        public bool AddMember(string memberName)
        {
            return MemberNames.Add(memberName); // Add returns false if the member already exists
        }

        public bool RemoveMember(string memberName)
        {
            return MemberNames.Remove(memberName); // Remove returns false if the member does not exist
        }
    }

    public class TeamManager : MonoBehaviour
    {
        private static Dictionary<GlobalEnums.GameTeam, TeamData> teams = new Dictionary<GlobalEnums.GameTeam, TeamData>();
        private static HashSet<string> globalMembers = new HashSet<string>();

        public static bool RegisterTeam(GlobalEnums.GameTeam teamId, Color teamColor, string teamName)
        {
            if (teamId == GlobalEnums.GameTeam.None || teams.ContainsKey(teamId))
            {
                Debug.LogWarning($"Team {teamId} is already registered or invalid.");
                return false;
            }

            teams[teamId] = new TeamData(teamColor, teamName);
            Debug.Log($"Team {teamName} registered successfully.");
            return true;
        }

        public static bool UnregisterTeam(GlobalEnums.GameTeam teamId)
        {
            if (teams.ContainsKey(teamId))
            {
                var team = teams[teamId];

                // Remove all members of the team from the global member list
                foreach (var member in team.MemberNames)
                {
                    globalMembers.Remove(member);
                }

                teams.Remove(teamId);
                Debug.Log($"Team {teamId} unregistered successfully.");
                return true;
            }

            Debug.LogWarning($"Team {teamId} is not registered.");
            return false;
        }

        public static bool AddMemberToTeam(GlobalEnums.GameTeam teamId, string memberName)
        {
            if (teams.TryGetValue(teamId, out var team))
            {
                if (globalMembers.Contains(memberName))
                {
                    Debug.LogWarning($"Member {memberName} is already part of a team.");
                    return false;
                }

                if (team.AddMember(memberName))
                {
                    globalMembers.Add(memberName); // Add to global member tracker
                    Debug.Log($"Member {memberName} added to team {team.TeamName}.");
                    return true;
                }

                Debug.LogWarning($"Member {memberName} is already in team {team.TeamName}.");
                return false;
            }

            Debug.LogWarning($"Team {teamId} is not registered.");
            return false;
        }

        public static bool RemoveMemberFromTeam(GlobalEnums.GameTeam teamId, string memberName)
        {
            if (teams.TryGetValue(teamId, out var team))
            {
                if (team.RemoveMember(memberName))
                {
                    globalMembers.Remove(memberName); // Remove from global member tracker
                    Debug.Log($"Member {memberName} removed from team {team.TeamName}.");
                    return true;
                }

                Debug.LogWarning($"Member {memberName} is not in team {team.TeamName}.");
                return false;
            }

            Debug.LogWarning($"Team {teamId} is not registered.");
            return false;
        }

        public static TeamData GetTeamData(GlobalEnums.GameTeam teamId)
        {
            if (teams.TryGetValue(teamId, out var team))
            {
                return team;
            }

            Debug.LogWarning($"Team {teamId} is not registered.");
            return null;
        }

        public static void ClearAllTeams()
        {
            teams.Clear();
            globalMembers.Clear();
            Debug.Log("All teams and members cleared.");
        }
    } //TeamManager
} //RedGaint