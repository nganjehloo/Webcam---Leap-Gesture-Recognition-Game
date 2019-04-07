using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Attributes;
using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCvSharp;

public class PlayerController : MonoBehaviour
{

    public int LIMIT_ANGLE_SUP = 60;
    public int LIMIT_ANGLE_INF = 5;
    public float BOUNDING_RECT_FINGER_SIZE_SCALING = 0.3f;
    public float BOUNDING_RECT_NEIGHBOR_DISTANCE_SCALING = 0.05f;
    public string finalPose;
    public int fingerCount;

    Mat background;
    Mat foreground;
    Mat skin;
    Texture2D finalImage;
    bool calibrated = false;
    public bool useML = true;
    public bool realTimeLearning = true;
    public bool deNoise = false;

    OpenCvSharp.BackgroundSubtractor bsub = null;

    OpenCvSharp.Rect skinColorSamplerRectangle1, skinColorSamplerRectangle2;


    OpenCvSharp.Rect backgroundColorSamplerRectangle1, backgroundColorSamplerRectangle2;
    OpenCvSharp.Rect backgroundColorSamplerRectangle3, backgroundColorSamplerRectangle4;

    int[] bHueL = new int[4];
    int[] bSatL = new int[4];
    int[] bvalL = new int[4];

    int[] bHueH = new int[4];
    int[] bSatH = new int[4];
    int[] bvalH = new int[4];

    int sHueL = 0;
    int sSatL = 0;
    int svalL = 0;

    int sHueH = 1;
    int sSatH = 1;
    int svalH = 1;

    void detectLeapGesture() {

    }

    void drawColorSampler(Mat input)
    {
        int frameWidth = input.Size().Width, frameHeight = input.Size().Height;

        int rectangleSize = 40;
        Scalar rectangleColor = new Scalar(255, 0, 255);
        
        skinColorSamplerRectangle1 = new OpenCvSharp.Rect( 160, frameHeight / 2, rectangleSize, rectangleSize);
        skinColorSamplerRectangle2 = new OpenCvSharp.Rect( 160, frameHeight / 3, rectangleSize, rectangleSize);

        Cv2.Rectangle(
            input,
            skinColorSamplerRectangle1,
            rectangleColor
        );

        Cv2.Rectangle(
            input,
            skinColorSamplerRectangle2,
            rectangleColor
        );

        backgroundColorSamplerRectangle1 = new OpenCvSharp.Rect(80, 80, rectangleSize, rectangleSize);
        backgroundColorSamplerRectangle2 = new OpenCvSharp.Rect(560, 80, rectangleSize, rectangleSize);
        backgroundColorSamplerRectangle3 = new OpenCvSharp.Rect(80, 400, rectangleSize, rectangleSize);
        backgroundColorSamplerRectangle4 = new OpenCvSharp.Rect(560, 400, rectangleSize, rectangleSize);

        Cv2.Rectangle(
            input,
            backgroundColorSamplerRectangle1,
            rectangleColor
        );

        Cv2.Rectangle(
            input,
            backgroundColorSamplerRectangle2,
            rectangleColor
        );

        Cv2.Rectangle(
            input,
            backgroundColorSamplerRectangle3,
            rectangleColor
        );

        Cv2.Rectangle(
            input,
            backgroundColorSamplerRectangle4,
            rectangleColor
        );


    }

    void calibrateBackground(Mat input)
    {

        Mat sample1 = new Mat(input, backgroundColorSamplerRectangle1);
        Mat sample2 = new Mat(input, backgroundColorSamplerRectangle2);
        Mat sample3 = new Mat(input, backgroundColorSamplerRectangle3);
        Mat sample4 = new Mat(input, backgroundColorSamplerRectangle4);

        calculateBackThreshold(sample1, sample2, sample3, sample4);


    }

    void calculateBackThreshold(Mat sample1, Mat sample2, Mat sample3, Mat sample4)
    {
        int offsetLowThreshold = 80;
        int offsetHighThreshold = 30;

        Scalar[] hsvMeanSamples = new Scalar[4];

        hsvMeanSamples[0] = Cv2.Mean(sample1);
        hsvMeanSamples[1] = Cv2.Mean(sample2);
        hsvMeanSamples[2] = Cv2.Mean(sample3);
        hsvMeanSamples[3] = Cv2.Mean(sample4);

        Scalar mean1 = Cv2.Mean(sample1);
        Scalar mean2 = Cv2.Mean(sample2);
        Scalar mean3 = Cv2.Mean(sample3);
        Scalar mean4 = Cv2.Mean(sample4);

        bHueL[0] = Convert.ToInt32(mean1.Val0) - offsetLowThreshold;
        bHueH[0] = Convert.ToInt32(mean1.Val0) + offsetHighThreshold;

        bSatL[0] = Convert.ToInt32(mean1.Val1) - offsetLowThreshold;
        bSatH[0] = Convert.ToInt32(mean1.Val1) + offsetHighThreshold;

        bvalL[0] = Convert.ToInt32(mean1.Val2) - offsetLowThreshold;
        bvalH[0] = Convert.ToInt32(mean1.Val2) + offsetHighThreshold;


        bHueL[1] = Convert.ToInt32(mean2.Val0) - offsetLowThreshold;
        bHueH[1] = Convert.ToInt32(mean2.Val0) + offsetHighThreshold;

        bSatL[1] = Convert.ToInt32(mean2.Val1) - offsetLowThreshold;
        bSatH[1] = Convert.ToInt32(mean2.Val1) + offsetHighThreshold;

        bvalL[1] = Convert.ToInt32(mean2.Val2) - offsetLowThreshold;
        bvalH[1] = Convert.ToInt32(mean2.Val2) + offsetHighThreshold;


        bHueL[2] = Convert.ToInt32(mean3.Val0) - offsetLowThreshold;
        bHueH[2] = Convert.ToInt32(mean3.Val0) + offsetHighThreshold;

        bSatL[2] = Convert.ToInt32(mean3.Val1) - offsetLowThreshold;
        bSatH[2] = Convert.ToInt32(mean3.Val1) + offsetHighThreshold;

        bvalL[2] = Convert.ToInt32(mean3.Val2) - offsetLowThreshold;
        bvalH[2] = Convert.ToInt32(mean3.Val2) + offsetHighThreshold;


        bHueL[3] = Convert.ToInt32(mean4.Val0) - offsetLowThreshold;
        bHueH[3] = Convert.ToInt32(mean4.Val0) + offsetHighThreshold;

        bSatL[3] = Convert.ToInt32(mean4.Val1) - offsetLowThreshold;
        bSatH[3] = Convert.ToInt32(mean4.Val1) + offsetHighThreshold;

        bvalL[3] = Convert.ToInt32(mean4.Val2) - offsetLowThreshold;
        bvalH[3] = Convert.ToInt32(mean4.Val2) + offsetHighThreshold;


        /*
        for ( int i = 0; i < 4; ++i)
        {
            bHueL[i] = Convert.ToInt32(hsvMeanSamples[i].Val0) - offsetLowThreshold;
            bHueL[i] = Convert.ToInt32(hsvMeanSamples[i].Val0) + offsetHighThreshold;

            bSatL[i] = Convert.ToInt32(hsvMeanSamples[i].Val1) - offsetLowThreshold;
            bSatL[i] = Convert.ToInt32(hsvMeanSamples[i].Val1) + offsetLowThreshold;

            bvalL[i] = Convert.ToInt32(hsvMeanSamples[i].Val2) - offsetLowThreshold;
            bvalH[i] = Convert.ToInt32(hsvMeanSamples[i].Val2) + offsetHighThreshold;

            Debug.Log("Sample " + i);
            Debug.Log("HueL: " + bHueL[i] + "SatL: " + bSatL[i] + "ValL: " + bvalL[i]);
            Debug.Log("HueH: " + bHueH[i] + " SatH: " + bSatH[i] + " ValH: " + bvalH[i]);
        }*/


    }

    Mat getBackgroundMask(Mat input)
    {
        Mat[] backMask = new Mat[4];
        Mat backMaskfin = new Mat();
        Mat dilate = new Mat();


        /*Cv2.InRange(
        input,
        new Scalar(bHueL[0], bSatL[0], bvalL[0]),
        new Scalar(bHueH[0], bSatH[0], bvalH[0]),
        backMask1);*/

        for (int i = 0; i < 4; ++i)
        {
            backMask[i] = new Mat();
            Cv2.InRange(
            input,
            new Scalar(bHueL[i], bSatL[i], bvalL[i]),
            new Scalar(bHueH[i], bSatH[i], bvalH[i]),
            backMask[i]);
        }

        Cv2.BitwiseOr(backMask[0], backMask[0], backMaskfin);
        Cv2.BitwiseOr(backMaskfin, backMask[1], backMaskfin);
        Cv2.BitwiseOr(backMaskfin, backMask[2], backMaskfin);
        Cv2.BitwiseOr(backMaskfin, backMask[3], backMaskfin);

        Mat stElement = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(8, 8));
        Cv2.MorphologyEx(backMaskfin, backMaskfin, MorphTypes.Open, stElement);
        Cv2.MorphologyEx(backMaskfin, backMaskfin, MorphTypes.Close, stElement);
        Cv2.Dilate(backMaskfin, backMaskfin, stElement, new Point(-1, -1), 3);

        return backMaskfin;

    }

    void calibrateSkin(Mat input)
    {
       // Mat hsvInput = input.Clone();
        //Cv2.CvtColor(input, hsvInput, ColorConversionCodes.BGR2HSV);

        Mat sample1 = new Mat(input, skinColorSamplerRectangle1);
        Mat sample2 = new Mat(input, skinColorSamplerRectangle2);

        calculateSkinThreshold(sample1, sample2);

      
    }

    void calculateSkinThreshold(Mat sample1, Mat sample2)
    {
        int offsetLowThreshold = 80;
        int offsetHighThreshold = 30;

        Scalar hsvMeansSample1 = Cv2.Mean(sample1);
        Scalar hsvMeansSample2 = Cv2.Mean(sample2);

        sHueL = Convert.ToInt32(Math.Min(hsvMeansSample1[0], hsvMeansSample2[0])) - offsetLowThreshold;
        sHueH = Convert.ToInt32(Math.Max(hsvMeansSample1[0], hsvMeansSample2[0])) + offsetHighThreshold;

        sSatL = Convert.ToInt32(Math.Min(hsvMeansSample1[1], hsvMeansSample2[1])) - offsetLowThreshold;
        sSatH = Convert.ToInt32(Math.Max(hsvMeansSample1[1], hsvMeansSample2[1])) + offsetHighThreshold;

        svalL = Convert.ToInt32(Math.Min(hsvMeansSample1[2], hsvMeansSample2[2])) - offsetLowThreshold;
        svalH = Convert.ToInt32(Math.Max(hsvMeansSample1[2], hsvMeansSample2[2])) + offsetHighThreshold;
    }

    Mat getSkinMask(Mat input)
    {
        Mat skinMask = new Mat();
        Mat dilate = new Mat();

        Cv2.InRange(
           input,
           new Scalar(sHueL, sSatL, svalL),
           new Scalar(sHueH, sSatH, svalH),
           skinMask);

        Mat stElement = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(8, 8));
        Cv2.MorphologyEx(skinMask, skinMask, MorphTypes.Open, stElement);

        Cv2.Erode(skinMask, skinMask, dilate, new Point(-1, -1), 2);


        return skinMask;

    }

    public Mat getHandContour(Mat input, Mat frame)
    {

        Mat detectImage = frame.Clone();
        if (input.Empty())
            return detectImage;

        // we work only on the 1 channel result, since this function is called inside a loop we are not sure that this is always the case
        if (input.Channels() != 1)
            return detectImage;

        Point[][] contours;
        HierarchyIndex[] hierarchy;

        Cv2.FindContours(input,
            out contours,
            out hierarchy,
            RetrievalModes.External,
            ContourApproximationModes.ApproxSimple);

        if (contours.Length <= 0)
            return detectImage;


        int biggest_contour_index = -1;
        double biggest_area = 0.0;

        for (int i = 0; i < contours.GetLength(0); i++)
        {
            double area = Cv2.ContourArea(contours[i], false);
            if (area > biggest_area)
            {
                biggest_area = area;
                biggest_contour_index = i;
            }
        }

        if (biggest_contour_index < 0)
            return detectImage;

       // Debug.Log(" " + biggest_area + " " + biggest_contour_index);
        Point[] hull_points;
        int[] hull_ints;


        hull_points = Cv2.ConvexHull(contours[biggest_contour_index]);
        hull_ints = Cv2.ConvexHullIndices(contours[biggest_contour_index]);
        //Debug.Log("hull " + hull_points.Length);
        //Debug.Log("hull " + hull_ints.Length);

        Vec4i[] defects;

        if (hull_ints.Length > 3)
            defects = Cv2.ConvexityDefects(contours[biggest_contour_index], hull_ints);
        else
            return detectImage;



        ///////////////
        OpenCvSharp.Rect bounding_rectangle = Cv2.BoundingRect( hull_points);

        // we find the center of the bounding rectangle, this should approximately also be the center of the hand
        Point center_bounding_rect = new Point(
            (bounding_rectangle.TopLeft.X + bounding_rectangle.BottomRight.X) / 2,
		(bounding_rectangle.TopLeft.Y + bounding_rectangle.BottomRight.Y) / 2
	    );


        // we separate the defects keeping only the ones of intrest
        List<Point> start_points = new List<Point>();
        List<Point> far_points = new List<Point>();

       // Debug.Log(" " + defects.Length);
        //Debug.Log(" " + contours[biggest_contour_index].Length);
        //Debug.Log(" " + contours.Length);

        
        for (int i = 0; i < defects.Length; i++)
        {
            
            start_points.Add(contours[biggest_contour_index][defects[i].Item0]);
            // filtering the far point based on the distance from the center of the bounding rectangle
            if (findPointsDistance(contours[biggest_contour_index][defects[i].Item2], center_bounding_rect) < bounding_rectangle.Height * BOUNDING_RECT_FINGER_SIZE_SCALING)
                far_points.Add(contours[biggest_contour_index][defects[i].Item2]);
        }

        // we compact them on their medians
        List<Point> filtered_start_points = compactOnNeighborhoodMedian(start_points, bounding_rectangle.Height * BOUNDING_RECT_NEIGHBOR_DISTANCE_SCALING);
        List<Point> filtered_far_points = compactOnNeighborhoodMedian(far_points, bounding_rectangle.Height * BOUNDING_RECT_NEIGHBOR_DISTANCE_SCALING);

        // now we try to find the fingers
        List<Point> filtered_finger_points = new List<Point>();

        if (filtered_far_points.Count > 1)
        {
            List<Point> finger_points = new List<Point>();

            for (int i = 0; i < filtered_start_points.Count; i++)
            {
                List<Point> closest_points = findClosestOnX(filtered_far_points, filtered_start_points[i]);

                if (isFinger(closest_points[0], filtered_start_points[i], closest_points[1], LIMIT_ANGLE_INF, LIMIT_ANGLE_SUP, center_bounding_rect, bounding_rectangle.Height * BOUNDING_RECT_FINGER_SIZE_SCALING))
                    finger_points.Add(filtered_start_points[i]);
            }

            if (finger_points.Count > 0)
            {

                // we have at most five fingers usually :)
                while (finger_points.Count > 5)
                    finger_points.RemoveAt(0);

                // filter out the points too close to each other
                for (int i = 0; i < finger_points.Count - 1; i++)
                {
                    if (findPointsDistanceOnX(finger_points[i], finger_points[i + 1]) > bounding_rectangle.Height * BOUNDING_RECT_NEIGHBOR_DISTANCE_SCALING * 1.5)
                        filtered_finger_points.Add(finger_points[i]);
                }

                if (finger_points.Count > 2)
                {
                    if (findPointsDistanceOnX(finger_points[0], finger_points[finger_points.Count - 1]) > bounding_rectangle.Height * BOUNDING_RECT_NEIGHBOR_DISTANCE_SCALING * 1.5)
                        filtered_finger_points.Add(finger_points[finger_points.Count - 1]);
                }
                else
                    filtered_finger_points.Add(finger_points[finger_points.Count - 1]);
            }
        }

        // we draw what found on the returned image 
        Cv2.DrawContours(detectImage, contours, biggest_contour_index, new Scalar(0, 255, 0), 8, LineTypes.Link8, hierarchy);
        //Cv2.Polylines(input, new IEnumerable<IEnumerable<Point>>( hull_points), true, new Scalar(1, 0, 0));
        Cv2.Rectangle(detectImage, bounding_rectangle.TopLeft, bounding_rectangle.BottomRight, new Scalar(0, 0, 255), 8, LineTypes.Link8);
        Cv2.Circle(detectImage, center_bounding_rect.X, center_bounding_rect.Y, 15, new Scalar(255, 0, 255), 3, LineTypes.Link8);
        drawVectorPoints(detectImage, filtered_start_points, new Scalar(255, 0, 0), true);
        drawVectorPoints(detectImage, filtered_far_points, new Scalar(0, 0, 25), true);
        drawVectorPoints(detectImage, filtered_finger_points, new Scalar(0, 255, 255), false);
        Cv2.PutText(detectImage, filtered_finger_points.Count.ToString(), center_bounding_rect,HersheyFonts.HersheyTriplex, 10, new Scalar(255, 0, 25));

        // and on the starting frame
        Cv2.DrawContours(frame, contours, biggest_contour_index, new Scalar(0, 255, 0), 8, LineTypes.Link8, hierarchy);
        Cv2.Circle(frame, center_bounding_rect.X, center_bounding_rect.Y, 15, new Scalar(255, 0, 255), 8, LineTypes.Link8);
        drawVectorPoints(frame, filtered_finger_points, new Scalar(0, 255, 255), false);
        Cv2.PutText(frame,filtered_finger_points.Count.ToString(), center_bounding_rect, HersheyFonts.HersheyTriplex, 10, new Scalar(255, 0, 255));

        fingerCount = filtered_finger_points.Count;
        if (fingerCount <= 1)
            finalPose = "rock";

        if (fingerCount == 2 && fingerCount <= 3)
            finalPose = "scissor";

        if (fingerCount >= 4)
            finalPose = "paper";

        return detectImage;
    }

    bool isFinger(Point a, Point b, Point c, double limit_angle_inf, double limit_angle_sup, Point palm_center, double min_distance_from_palm)
    {
        double angle = findAngle(a, b, c);
        if (angle > limit_angle_sup || angle < limit_angle_inf)
            return false;

        // the finger point sohould not be under the two far points
        int delta_y_1 = b.Y - a.Y;
        int delta_y_2 = b.Y - c.Y;
        if (delta_y_1 > 0 && delta_y_2 > 0)
            return false;

        // the two far points should not be both under the center of the hand
        int delta_y_3 = palm_center.Y - a.Y;
        int delta_y_4 = palm_center.Y - c.Y;
        if (delta_y_3 < 0 && delta_y_4 < 0)
            return false;

        double distance_from_palm = findPointsDistance(b, palm_center);
        if (distance_from_palm < min_distance_from_palm)
            return false;

        // this should be the case when no fingers are up
        double distance_from_palm_far_1 = findPointsDistance(a, palm_center);
        double distance_from_palm_far_2 = findPointsDistance(c, palm_center);
        if (distance_from_palm_far_1 < min_distance_from_palm / 4 || distance_from_palm_far_2 < min_distance_from_palm / 4)
            return false;

        return true;
    }

    double findAngle(Point a, Point b, Point c)
    {
        double ab = findPointsDistance(a, b);
        double bc = findPointsDistance(b, c);
        double ac = findPointsDistance(a, c);
        return Math.Acos((ab * ab + bc * bc - ac * ac) / (2 * ab * bc)) * 180 / Cv2.PI;
    }

    List<Point> findClosestOnX(List<Point> points, Point pivot)
    {
        List<Point> to_return = new List<Point>();
        to_return.Add(new Point());
        to_return.Add(new Point());

        if (points.Count == 0)
            return to_return;

        double distance_x_1 = Double.MaxValue;
        double distance_1 = Double.MaxValue;
        double distance_x_2 = Double.MaxValue;
        double distance_2 = Double.MaxValue;
        int index_found = 0;

        for (int i = 0; i < points.Count; i++)
        {
            double distance_x = findPointsDistanceOnX(pivot, points[i]);
            double distance = findPointsDistance(pivot, points[i]);

            if (distance_x < distance_x_1 && distance_x != 0 && distance <= distance_1)
            {
                distance_x_1 = distance_x;
                distance_1 = distance;
                index_found = i;
            }
        }

       // Debug.Log("to Return: " + to_return.Count);
        //Debug.Log("points: " + points.Count + " index " + index_found);
        to_return[0] = points[index_found];

        for (int i = 0; i < points.Count; i++)
        {
            double distance_x = findPointsDistanceOnX(pivot, points[i]);
            double distance = findPointsDistance(pivot, points[i]);

            if (distance_x < distance_x_2 && distance_x != 0 && distance <= distance_2 && distance_x != distance_x_1)
            {
                distance_x_2 = distance_x;
                distance_2 = distance;
                index_found = i;
            }
        }

        to_return[1] = points[index_found];

        return to_return;
    }

    double findPointsDistanceOnX(Point a, Point b)
    {
        double to_return = 0.0;

        if (a.X > b.X)
            to_return = a.X - b.X;
        else
            to_return = b.X - a.X;

        return to_return;
    }

    void drawVectorPoints(Mat image, List<Point> points, Scalar color, bool with_numbers)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Cv2.Circle(image, points[i].X, points[i].Y, 15, color, 5, LineTypes.Link8);
            if (with_numbers)
                Cv2.PutText(image, i.ToString(), points[i], HersheyFonts.HersheyTriplex, 2, color);
        }
    }

    List<Point> compactOnNeighborhoodMedian(List<Point> points, double max_neighbor_distance)
    {
        List<Point> median_points = new List<Point>();

        if (points.Capacity == 0)
            return median_points;

        if (max_neighbor_distance <= 0)
            return median_points;

        // we start with the first point
        Point reference = points[0];
        Point median = points[0];

        for (int i = 1; i < points.Count; i++)
        {
            if (findPointsDistance(reference, points[i]) > max_neighbor_distance)
            {

                // the point is not in range, we save the median
                median_points.Add(median);

                // we swap the reference
                reference = points[i];
                median = points[i];
            }
            else
                median = (points[i] + median) / 2;
        }

        // last median
        median_points.Add(median);

        return median_points;
    }

    double findPointsDistance(Point a, Point b)
    {
        Point difference = a - b;
        return Math.Sqrt(difference.DotProduct(difference));
    }

    public void proessHand(UnityEngine.Texture texture, ref Texture2D output)
    {
        Mat processingImage = OpenCvSharp.Unity.TextureToMat(texture as WebCamTexture);
        Mat frameCopy = processingImage.Clone();
        Cv2.CvtColor(processingImage, processingImage, ColorConversionCodes.BGR2HSV);
        Mat maskedImage = new Mat();
        Mat maskedSkin = new Mat();
        Mat maskedBackground = new Mat();

        if (Input.GetKey(KeyCode.M))
            useML = !useML;
        if (Input.GetKey(KeyCode.R))
            realTimeLearning = !realTimeLearning;
        if (Input.GetKey(KeyCode.N))
            deNoise = !deNoise;

        if(deNoise)
            Cv2.FastNlMeansDenoising(processingImage, processingImage, 1, 2, 5);

        if (!useML)
        { 
            
            drawColorSampler(processingImage);

            if (Input.GetKey(KeyCode.B))
                calibrateBackground(frameCopy);

            if (Input.GetKey(KeyCode.S))
                calibrateSkin(frameCopy);

            background = getBackgroundMask(frameCopy);
            skin = getSkinMask(frameCopy);

            Cv2.BitwiseNot(background, background);
            Cv2.BitwiseOr(skin, background, foreground);

            Mat debug = getHandContour(skin, frameCopy);

            Cv2.BitwiseAnd(processingImage, processingImage, maskedSkin, skin);

            Cv2.BitwiseAnd(processingImage, processingImage, maskedBackground, background);

            Cv2.BitwiseAnd(frameCopy, frameCopy, maskedImage, foreground);

            Cv2.CvtColor(maskedImage, maskedImage, ColorConversionCodes.HSV2BGR);
            Cv2.CvtColor(maskedSkin, maskedSkin, ColorConversionCodes.HSV2BGR);
            Cv2.CvtColor(maskedBackground, maskedBackground, ColorConversionCodes.HSV2BGR);
           // Cv2.CvtColor(debug, debug, ColorConversionCodes.HSV2BGR);

            if (Input.GetKey(KeyCode.Q))
                output = OpenCvSharp.Unity.MatToTexture(maskedSkin, output);
            else if (Input.GetKey(KeyCode.W))
                output = OpenCvSharp.Unity.MatToTexture(maskedBackground, output);
            else
                output = OpenCvSharp.Unity.MatToTexture(debug, output);
        }
        else
        {
            background = getBackgroundMask(processingImage);
            if (realTimeLearning)
                bsub.Apply(processingImage, background);
            else
                bsub.Apply(processingImage, background, 0);

            Mat debug = null;
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(8, 8));
            Cv2.MorphologyEx(background, background, MorphTypes.Open, kernel);
            Cv2.MorphologyEx(background, background, MorphTypes.Open, kernel);
            Cv2.MorphologyEx(background, background, MorphTypes.Close, kernel);
            //Cv2.Dilate(background, background, kernel, new Point(-1, -1), 1);
            Cv2.BitwiseAnd(processingImage, processingImage, maskedBackground, background);

            debug = getHandContour(background, processingImage);
           // Cv2.CvtColor(maskedBackground, maskedBackground, ColorConversionCodes.HSV2BGR);
            //Cv2.CvtColor(processingImage, processingImage, ColorConversionCodes.HSV2BGR);
            Cv2.CvtColor(debug, debug, ColorConversionCodes.HSV2BGR);
            Cv2.CvtColor(background, background, ColorConversionCodes.GRAY2BGR);
            if(Input.GetKey(KeyCode.E))
                output = OpenCvSharp.Unity.MatToTexture(background, output);
            else
                output = OpenCvSharp.Unity.MatToTexture(debug, output);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        finalPose = "rock";
        background = new Mat();
        skin = new Mat();
        foreground = new Mat();
        bsub = OpenCvSharp.BackgroundSubtractorKNN.Create(500, 250, true);

    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
