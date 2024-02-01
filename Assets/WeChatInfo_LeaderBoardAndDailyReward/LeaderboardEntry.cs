

using System;
using UnityEngine;
using UnityEngine.UI;

namespace PiratePanic
{
	/// <summary>
	/// Single leaderboard record UI.
	/// </summary>
	public class LeaderboardEntry : MonoBehaviour
	{
		/// <summary>
		/// Textbox displaying user rank.
		/// </summary>
		[SerializeField] private Text _rank = null;

		/// <summary>
		/// Textbox containing displayed user's name.
		/// </summary>
		[SerializeField] private Text _username = null;

		/// <summary>
		/// Textbox containing displayed user's score.
		/// </summary>
		[SerializeField] private Text _score = null;

		/*/// <summary>
		/// On click shows displayed user's profile.
		/// </summary>
		[SerializeField] private Button _profile = null;*/

		/// <summary>
		/// Sets the UI of this leaderboard entry.
		/// </summary>
		public void SetPlayer(string username, int rank, string score)
		{
			_username.text = username;
			_rank.text = rank + ".";
			_score.text = score;
			//_profile.onClick.AddListener(() => onProfileClicked());
		}
	}
}
