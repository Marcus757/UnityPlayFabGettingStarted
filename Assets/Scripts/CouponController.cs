using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class CouponController : MonoBehaviour
{
    public void RedeemCoupon(string username, string password, string couponCode)
    {
        var loginWithPlayFabRequest = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest,
            delegate (LoginResult loginResult)
            {
                var redeemCouponRequest = new RedeemCouponRequest() { CouponCode = couponCode };

                PlayFabClientAPI.RedeemCoupon(redeemCouponRequest,
                    delegate (RedeemCouponResult redeemCouponResult)
                    {
                        Debug.Log("Coupon code: " + couponCode + " has been redeemed");
                    },
                    SharedError.OnSharedError);
            },
            SharedError.OnSharedError);
    }
}
