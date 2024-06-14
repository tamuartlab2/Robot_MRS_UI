
using RosSharp.RosBridgeClient.MessageTypes.Std;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;

namespace RosSharp.RosBridgeClient.MessageTypes.Nav
{
    public class Odometry_ArtLab : Message
    {
        public const string RosMessageName = "nav_msgs/Odometry";

        //  This represents an estimate of a position and velocity in free space.  
        //  The pose in this message should be specified in the coordinate frame given by header.frame_id.
        //  The twist in this message should be specified in the coordinate frame given by the child_frame_id
        public Header_ArtLab header { get; set; }
        public string child_frame_id { get; set; }
        public PoseWithCovariance pose { get; set; }
        public TwistWithCovariance twist { get; set; }

        public Odometry_ArtLab()
        {
            this.header = new Header_ArtLab();
            this.child_frame_id = "";
            this.pose = new PoseWithCovariance();
            this.twist = new TwistWithCovariance();
        }

        public Odometry_ArtLab(Header_ArtLab header, string child_frame_id, PoseWithCovariance pose, TwistWithCovariance twist)
        {
            this.header = header;
            this.child_frame_id = child_frame_id;
            this.pose = pose;
            this.twist = twist;
        }
    }
}